using AppCore.Context;
using AppCore.Models;
using AppCore.Property;
using AppCore.Services.NecessaryDataClass;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Services
{
    public class NecessaryData
    {
        private bool _need_to_save = false;
        private readonly FTDbContext context;

        public NecessaryData(FTDbContext context)
        {
            this.context = context;
        }

        public async Task<bool> CheckDataIntegrity()
        {
            CheckObjets();
            CheckRessources();
            CheckConstructionInfo();
            if (_need_to_save)
            {
                await context.SaveChangesAsync();
            }

            return true;
        }

        private void Add<T>(in DbSet<T> list, in T data) where T : class
        {
            if(!_need_to_save)
            {
                _need_to_save = true;
            }
            list.Add(data);
        }

        private void Check<T>(in DbSet<T> list, IEnumerable<T> basicData, Func<DbSet<T>, T,bool> checkIsOk) where T : class
        {
            foreach(T data in basicData)
            {
                if (!checkIsOk.Invoke(list, data))
                {
                    Add(in list, in data);
                }
            }
        }

        #region OBJETS

        private void CheckObjets()
        {
            Check(context.Objets, objets, 
                (dbSet, data) => dbSet.FirstOrDefault(x => x.ObjetId == data.ObjetId) != null);
        }

        private static readonly Objet[] objets = 
        {
            new Objet(){ObjetId = 1, Rarete = TypeRarete.COMMUN, Nom = "Fil de fer"},
            new Objet(){ObjetId = 2, Rarete = TypeRarete.EPIC, Nom = "Batterie"},
            new Objet(){ObjetId = 3, Rarete = TypeRarete.LEGENDAIRE, Nom = "Moteur"},
            new Objet(){ObjetId = 4, Rarete = TypeRarete.RARE, Nom = "Ciment"},
            new Objet(){ObjetId = 5, Rarete = TypeRarete.COMMUN, Nom = "Argile"},
            new Objet(){ObjetId = 6, Rarete = TypeRarete.RARE, Nom = "Ressort"},
            new Objet(){ObjetId = 7, Rarete = TypeRarete.COMMUN, Nom = "Layton"},
        };

        private static Objet GetObjet(string name) => objets.First(x => x.Nom == name);

        public static Objet GetObjet(OBJET obj) => objets[(int)obj];

        public enum OBJET
        {
            FIL_DE_FER = 0,
            BATTERIE = 1,
            MOTEUR = 2,
            CIMENT = 3,
            ARGILE = 4,
            RESSORT = 5,
            LAYTON = 6,
        }

        #endregion

        #region RESSOURCES

        private void CheckRessources()
        {
            Check(context.Ressources, ressources,
                (dbSet, data) => dbSet.FirstOrDefault(x => x.RessourceId == data.RessourceId) != null);
        }

        private static readonly Ressource[] ressources =
        {
            new Ressource(){RessourceId = 1, Nom = "Eau"},
            new Ressource(){RessourceId = 2, Nom = "Nourriture"},
            new Ressource(){RessourceId = 3, Nom = "Bonheur"},
            new Ressource(){RessourceId = 4, Nom = "Energie"},
            new Ressource(){RessourceId = 5, Nom = "Bois"},
            new Ressource(){RessourceId = 6, Nom = "Ferraille"},
            new Ressource(){RessourceId = 7, Nom = "Habitant"},
        };

        private static Ressource GetRessource(string name) => ressources.First(x => x.Nom == name);

        public static Ressource GetRessource(RESSOURCE res) => ressources[(int)res];

        public enum RESSOURCE
        {
            EAU = 0,
            NOURRITURE = 1,
            BONHEUR = 2,
            ENERGIE = 3,
            BOIS = 4,
            FERRAILLE = 5,
            HABITANT = 6,
        }

        #endregion

        #region CONSTRUCTION_INFO

        private void CheckConstructionInfo()
        {
            foreach(var schema in constructionInfoSchemas) { 
                if(context.ConstructionInfos.FirstOrDefault(x => x.ConsInfoId == schema.ConsInfoId) == null)
                {
                    // infos de base
                    Add(context.ConstructionInfos,
                        new () { 
                            ConsInfoId = schema.ConsInfoId,
                            Nom = schema.Nom,
                            Type = schema.Type,
                            VieMax = schema.VieMax,
                            NiveauMax = schema.NiveauMax,
                            TempsSecConstruction = schema.TempsSecConstruction,
                        });

                    // mise en place des schémas de constructions
                    foreach(var itemSchema in schema.CreationsRessources)
                    {
                        Add(context.CreationObjets, new()
                        {
                            ConsInfoId = schema.ConsInfoId,
                            Type = schema.Type,
                            ObjetId = itemSchema.Key,
                            Nombre = itemSchema.Value
                        });
                    }
                    foreach (var itemSchema in schema.CreationsRessources)
                    {
                        Add(context.CreationRessources, new()
                        {
                            ConsInfoId = schema.ConsInfoId,
                            Type = schema.Type,
                            RessourceId = itemSchema.Key,
                            Nombre = itemSchema.Value,
                        });
                    }

                    // mise en place des schémas d'amélioration
                    foreach(var itemSchema in schema.AmeliorationsRessources)
                    {
                        foreach(var dataSchema in itemSchema.Value)
                        {
                            Add(context.AmeliorationRessources, new()
                            {
                                ConsInfoId = schema.ConsInfoId,
                                Type = schema.Type,
                                RessourceId = dataSchema.Key,
                                Nombre = dataSchema.Value,
                                NiveauConcerne = (byte)itemSchema.Key
                            });
                        }
                    }
                    foreach (var itemSchema in schema.AmeliorationsObjet)
                    {
                        foreach (var dataSchema in itemSchema.Value)
                        {
                            Add(context.AmeliorationObjets, new()
                            {
                                ConsInfoId = schema.ConsInfoId,
                                Type = schema.Type,
                                ObjetId = dataSchema.Key,
                                Nombre = dataSchema.Value,
                                NiveauConcerne = (byte)itemSchema.Key
                            });
                        }
                    }

                    // mise en place des schémas de réparations
                    foreach (var itemSchema in schema.ReparationsRessources)
                    {
                        Add(context.ReparationRessources, new()
                        {
                            ConsInfoId = schema.ConsInfoId,
                            Type = schema.Type,
                            RessourceId = itemSchema.Key,
                            Nombre = itemSchema.Value.Item1,
                            MultParNiveau = itemSchema.Value.Item2,
                            TempsSec = itemSchema.Value.Item3,
                        });
                    }
                }
            }
        }

        private readonly ConstructionInfoSchema[] constructionInfoSchemas =
        {
            ConstructionInfoSchema.CreateDef(1, "Tourelle Auto", 100, TypeConstruction.DEFENSE, 187200, 2.4f, 150) // données initial
                // schémas de créations
                .WithCreationsObjects(GetObjet(OBJET.FIL_DE_FER),GetObjet(OBJET.BATTERIE))
                .AddCreationRessources(GetRessource(RESSOURCE.ENERGIE), 50)
                .AddCreationRessources(GetRessource(RESSOURCE.FERRAILLE), 60)
                .AddCreationRessources(GetRessource(RESSOURCE.HABITANT), 5)
                // schémas d'améliorations
                .AddLevel(new []{ // niveau 2 
                    GetRessource(RESSOURCE.FERRAILLE)
                },new []{
                    50
                })
                .AddLevel(new []{ // niveau 3
                    GetRessource(RESSOURCE.FERRAILLE)
                },new []{
                    170
                })
                .AddLevel(new []{ // niveau 4
                    GetRessource(RESSOURCE.FERRAILLE)
                },new []{
                    350
                })
                // schéma de réparations
                .AddReparationRessources(GetRessource(RESSOURCE.FERRAILLE), 10, 1.2f, 4500),

            ConstructionInfoSchema.CreateDef(2, "Barrière", 120, TypeConstruction.DEFENSE, 3600, 1.4f, 20)
                .AddCreationRessources(GetRessource(RESSOURCE.BOIS),100)
                .AddLevel(new[] { // niveau 2
                    GetRessource(RESSOURCE.BOIS)
                }, new[] {
                    110
                })
                .AddLevel(new[] { // niveau 3
                    GetRessource(RESSOURCE.BOIS),
                    GetRessource(RESSOURCE.FERRAILLE)
                }, new[] {
                    160,
                    30
                })
                .AddLevel(new[] { // niveau 4
                    GetRessource(RESSOURCE.BOIS),
                    GetRessource(RESSOURCE.FERRAILLE)
                }, new[] {
                    200,
                    150
                })
                .AddLevel(new[] { // niveau 5
                    GetRessource(RESSOURCE.BOIS),
                    GetRessource(RESSOURCE.FERRAILLE)
                }, new[] {
                    150,
                    350
                })
                .AddLevel(new[] { // niveau 6
                    GetRessource(RESSOURCE.FERRAILLE)
                }, new[] {
                    800
                })
   .AddReparationRessources(GetRessource(RESSOURCE.BOIS), 20, 1.5f, 1200),

                        ConstructionInfoSchema.CreateDef(3, "Barbelé", 120, TypeConstruction.DEFENSE, 3600, 1.4f, 20)
                .AddCreationRessources(GetRessource(RESSOURCE.FERRAILLE),100)
                .AddLevel(new[] { // niveau 2
                    GetRessource(RESSOURCE.FERRAILLE)
                }, new[] {
                    110
                })
                .AddLevel(new[] { // niveau 3
                    GetRessource(RESSOURCE.FERRAILLE)
                }, new[] {
                    160,
                })
                .AddLevel(new[] { // niveau 4
                    GetRessource(RESSOURCE.FERRAILLE)
                }, new[] {
                    200,
                })
                .AddLevel(new[] { // niveau 5
                    GetRessource(RESSOURCE.FERRAILLE)
                }, new[] {
                    150,
                })
                .AddLevel(new[] { // niveau 6
                    GetRessource(RESSOURCE.FERRAILLE)
                }, new[] {
                    800
                })
   .AddReparationRessources(GetRessource(RESSOURCE.BOIS), 20, 1.5f, 1200),

                ConstructionInfoSchema.CreateDef(4, "Tour de Garde", 150, TypeConstruction.DEFENSE, 4800, 1.8f, 30)
    .AddCreationRessources(GetRessource(RESSOURCE.BOIS), 200)
    .AddCreationRessources(GetRessource(RESSOURCE.FERRAILLE), 150)
    .AddLevel(new[] { GetRessource(RESSOURCE.BOIS), GetRessource(RESSOURCE.FERRAILLE) }, new[] { 220, 50 })
    .AddLevel(new[] { GetRessource(RESSOURCE.BOIS), GetRessource(RESSOURCE.FERRAILLE) }, new[] { 300, 120 })
    .AddLevel(new[] { GetRessource(RESSOURCE.BOIS), GetRessource(RESSOURCE.FERRAILLE) }, new[] { 400, 250 })
    .AddLevel(new[] { GetRessource(RESSOURCE.BOIS), GetRessource(RESSOURCE.FERRAILLE) }, new[] { 550, 400 })
    .AddLevel(new[] { GetRessource(RESSOURCE.FERRAILLE) }, new[] { 1200 })
    .AddReparationRessources(GetRessource(RESSOURCE.BOIS), 30, 1.6f, 1800),

    ConstructionInfoSchema.CreateDef(5, "Canon", 400, TypeConstruction.DEFENSE, 14400, 2.5f, 150)
        .AddCreationRessources(GetRessource(RESSOURCE.FERRAILLE), 300)
        .AddLevel(new[] { GetRessource(RESSOURCE.FERRAILLE), GetRessource(RESSOURCE.BOIS) }, new[] { 400, 200 })
        .AddLevel(new[] { GetRessource(RESSOURCE.FERRAILLE), GetRessource(RESSOURCE.BOIS) }, new[] { 600, 400 })
        .AddLevel(new[] { GetRessource(RESSOURCE.FERRAILLE), GetRessource(RESSOURCE.BOIS) }, new[] { 900, 600 })
        .AddLevel(new[] { GetRessource(RESSOURCE.FERRAILLE), GetRessource(RESSOURCE.BOIS) }, new[] { 1200, 1000 })
        .AddLevel(new[] { GetRessource(RESSOURCE.FERRAILLE) }, new[] { 2000 })
        .AddReparationRessources(GetRessource(RESSOURCE.BOIS), 50, 2.3f, 200),


    ConstructionInfoSchema.CreateDef(6, "Centrale électrique", 200, TypeConstruction.ENERGIE, 6000, 1.8f, 30)
        .AddCreationRessources(GetRessource(RESSOURCE.FERRAILLE), 100)
        .AddLevel(new[] { GetRessource(RESSOURCE.FERRAILLE), GetRessource(RESSOURCE.BOIS) }, new[] { 150, 80 })
        .AddLevel(new[] { GetRessource(RESSOURCE.FERRAILLE), GetRessource(RESSOURCE.BOIS) }, new[] { 200, 120 })
        .AddLevel(new[] { GetRessource(RESSOURCE.FERRAILLE), GetRessource(RESSOURCE.BOIS) }, new[] { 250, 160 })
        .AddLevel(new[] { GetRessource(RESSOURCE.FERRAILLE), GetRessource(RESSOURCE.BOIS) }, new[] { 520, 450 })
        .AddLevel(new[] { GetRessource(RESSOURCE.FERRAILLE) }, new[] { 1000 })
        .AddReparationRessources(GetRessource(RESSOURCE.FERRAILLE), 20,  1.5f, 100),

ConstructionInfoSchema.CreateDef(7, "Ferme", 150, TypeConstruction.NOURRITURE, 4800, 1.5f, 25)
    .AddCreationRessources(GetRessource(RESSOURCE.BOIS), 80)
    .AddLevel(new[] { GetRessource(RESSOURCE.BOIS) }, new[] { 100 })
    .AddLevel(new[] { GetRessource(RESSOURCE.BOIS) }, new[] { 200 })
    .AddLevel(new[] { GetRessource(RESSOURCE.BOIS) }, new[] { 350 })
    .AddLevel(new[] { GetRessource(RESSOURCE.BOIS) }, new[] { 450 })
    .AddLevel(new[] { GetRessource(RESSOURCE.BOIS), GetRessource(RESSOURCE.FERRAILLE) }, new[] { 150, 50 })
    .AddReparationRessources(GetRessource(RESSOURCE.BOIS), 15, 1.2f, 100),

ConstructionInfoSchema.CreateDef(8, "Scierie", 180, TypeConstruction.ENERGIE, 5400, 1.7f, 35)
    .AddCreationRessources(GetRessource(RESSOURCE.FERRAILLE), 120)
    .AddLevel(new[] { GetRessource(RESSOURCE.FERRAILLE),GetRessource(RESSOURCE.BOIS) }, new[] { 150, 150 })
    .AddLevel(new[] { GetRessource(RESSOURCE.FERRAILLE),GetRessource(RESSOURCE.BOIS) }, new[] { 180, 220 })
    .AddLevel(new[] { GetRessource(RESSOURCE.FERRAILLE), GetRessource(RESSOURCE.BOIS) }, new[] { 220, 80 })
    .AddLevel(new[] { GetRessource(RESSOURCE.FERRAILLE), GetRessource(RESSOURCE.BOIS) }, new[] { 300, 320 })
    .AddLevel(new[] { GetRessource(RESSOURCE.FERRAILLE), GetRessource(RESSOURCE.BOIS) }, new[] { 450, 500 })
    .AddReparationRessources(GetRessource(RESSOURCE.FERRAILLE), 25, 1.3f, 100),

                ConstructionInfoSchema.CreateDef(9, "Usine de traitement d'eau", 200, TypeConstruction.EAU, 6000, 1.8f, 30)
    .AddCreationRessources(GetRessource(RESSOURCE.FERRAILLE), 150)
    .AddCreationRessources(GetRessource(RESSOURCE.BOIS), 100)
    .AddLevel(new[] { GetRessource(RESSOURCE.FERRAILLE), GetRessource(RESSOURCE.BOIS) }, new[] { 180, 80 })
    .AddLevel(new[] { GetRessource(RESSOURCE.FERRAILLE), GetRessource(RESSOURCE.BOIS) }, new[] { 220, 120 })
    .AddLevel(new[] { GetRessource(RESSOURCE.FERRAILLE), GetRessource(RESSOURCE.BOIS) }, new[] { 260, 160 })
    .AddLevel(new[] { GetRessource(RESSOURCE.FERRAILLE), GetRessource(RESSOURCE.BOIS) }, new[] { 300, 250 })
    .AddLevel(new[] { GetRessource(RESSOURCE.FERRAILLE), GetRessource(RESSOURCE.BOIS) }, new[] { 450, 500 })
    .AddReparationRessources(GetRessource(RESSOURCE.FERRAILLE), 30, 1.2f, 300)
    .AddReparationRessources(GetRessource(RESSOURCE.BOIS), 20, 1.5f, 1200),

                ConstructionInfoSchema.CreateDef(10, "Quartier résidentiel", 250, TypeConstruction.HABITATION, 7200, 2.0f, 40)
    .AddCreationRessources(GetRessource(RESSOURCE.BOIS), 200)
    .AddLevel(new[] { GetRessource(RESSOURCE.BOIS) }, new[] { 250 })
    .AddLevel(new[] { GetRessource(RESSOURCE.BOIS) }, new[] { 300 })
    .AddLevel(new[] { GetRessource(RESSOURCE.BOIS) }, new[] { 420 })
    .AddLevel(new[] { GetRessource(RESSOURCE.BOIS) }, new[] { 500 })
    .AddLevel(new[] { GetRessource(RESSOURCE.BOIS) }, new[] { 600 })
    .AddLevel(new[] { GetRessource(RESSOURCE.BOIS), GetRessource(RESSOURCE.FERRAILLE) }, new[] { 600, 600 })
    .AddReparationRessources(GetRessource(RESSOURCE.BOIS), 40, 1.3f, 400),

                ConstructionInfoSchema.CreateDef(11, "Fonderie", 180, TypeConstruction.ENERGIE, 5400, 1.7f, 35)
    .AddCreationRessources(GetRessource(RESSOURCE.BOIS), 120)
    .AddLevel(new[] { GetRessource(RESSOURCE.BOIS) }, new[] { 150 })
    .AddLevel(new[] { GetRessource(RESSOURCE.BOIS) }, new[] { 180 })
    .AddLevel(new[] { GetRessource(RESSOURCE.BOIS), GetRessource(RESSOURCE.FERRAILLE) }, new[] { 220, 80 })
    .AddLevel(new[] { GetRessource(RESSOURCE.BOIS), GetRessource(RESSOURCE.FERRAILLE) }, new[] { 280, 120 })
    .AddLevel(new[] { GetRessource(RESSOURCE.BOIS), GetRessource(RESSOURCE.FERRAILLE) }, new[] { 350, 200 })
    .AddReparationRessources(GetRessource(RESSOURCE.BOIS), 25, 1.3f, 350),

                ConstructionInfoSchema.CreateDef(12, "Caserne", 220, TypeConstruction.ATTAQUANT, 6600, 1.9f, 30)
    .AddCreationRessources(GetRessource(RESSOURCE.FERRAILLE), 180)
    .AddCreationRessources(GetRessource(RESSOURCE.BOIS), 100)
    .AddLevel(new[] { GetRessource(RESSOURCE.FERRAILLE), GetRessource(RESSOURCE.BOIS) }, new[] { 200, 80 })
    .AddLevel(new[] { GetRessource(RESSOURCE.FERRAILLE), GetRessource(RESSOURCE.BOIS) }, new[] { 240, 120 })
    .AddLevel(new[] { GetRessource(RESSOURCE.FERRAILLE), GetRessource(RESSOURCE.BOIS) }, new[] { 300, 160 })
    .AddLevel(new[] { GetRessource(RESSOURCE.FERRAILLE), GetRessource(RESSOURCE.BOIS) }, new[] { 350, 200 })
    .AddLevel(new[] { GetRessource(RESSOURCE.FERRAILLE), GetRessource(RESSOURCE.BOIS) }, new[] { 450, 300 })
    .AddReparationRessources(GetRessource(RESSOURCE.FERRAILLE), 35, 1.2f, 300)
    .AddReparationRessources(GetRessource(RESSOURCE.BOIS), 25, 1.5f, 300),

                ConstructionInfoSchema.CreateDef(13, "Théâtre", 200, TypeConstruction.DIVERTISSEMENT, 6000, 1.8f, 40)
    .AddCreationRessources(GetRessource(RESSOURCE.BOIS), 150)
    .AddCreationRessources(GetRessource(RESSOURCE.FERRAILLE), 100)
    .AddLevel(new[] { GetRessource(RESSOURCE.FERRAILLE), GetRessource(RESSOURCE.BOIS) }, new[] { 200, 80 })
    .AddLevel(new[] { GetRessource(RESSOURCE.FERRAILLE), GetRessource(RESSOURCE.BOIS) }, new[] { 240, 120 })
    .AddLevel(new[] { GetRessource(RESSOURCE.FERRAILLE), GetRessource(RESSOURCE.BOIS) }, new[] { 300, 160 })
    .AddLevel(new[] { GetRessource(RESSOURCE.FERRAILLE), GetRessource(RESSOURCE.BOIS) }, new[] { 350, 200 })
    .AddLevel(new[] { GetRessource(RESSOURCE.FERRAILLE), GetRessource(RESSOURCE.BOIS) }, new[] { 450, 300 })
    .AddReparationRessources(GetRessource(RESSOURCE.BOIS), 30, 1.2f, 300)
    .AddReparationRessources(GetRessource(RESSOURCE.FERRAILLE), 20, 1.5f, 300)


    };

        #endregion
    }
    
}
