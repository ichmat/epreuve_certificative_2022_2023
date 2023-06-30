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
        };

        #endregion
    }
    
}
