using AppCore.Models;
using AppCore.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AppCore.Context
{
    public class FTDbContext : DbContext
    {
        public DbSet<Utilisateur> Utilisateurs { get; set; }

        public DbSet<Stat> Stats { get; set; }

        public DbSet<Village> Villages { get; set; }

        public DbSet<Attaque> Attaques { get; set; }

        public DbSet<ConstructionInfo> ConstructionInfos { get; set; }

        //public DbSet<Construction> Constructions { get; set; }

        public DbSet<Ressource> Ressources { get; set; }

        public DbSet<Objet> Objets { get; set; }

        public DbSet<RessourcePossede> RessourcePossedes { get; set; }

        public DbSet<ObjetsPossede> ObjetsPossedes { get; set; }

        public DbSet<AmeliorationObjet> AmeliorationObjets { get; set; }

        public DbSet<AmeliorationRessource> AmeliorationRessources { get; set; }

        public DbSet<CreationObjet> CreationObjets { get; set; }

        public DbSet<CreationRessource> CreationRessources { get; set; }

        public DbSet<ReparationRessource> ReparationRessources { get; set; }

        public DbSet<ConstructionDef> ConstructionDefs { get; set; }

        public DbSet<ConstructionProd> ConstructionProds { get; set; }

        public DbSet<ConstructionAutre> ConstructionAutres { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var serverVersion = new MySqlServerVersion(new Version(5, 7, 36));
            var connectionString = "server=localhost;port=3306;user=root;password=;database=freshtech";
            optionsBuilder.UseMySql(connectionString,serverVersion, options =>
            {
                options.MigrationsAssembly("WebApplicationAPI");
            });
            base.OnConfiguring(optionsBuilder);
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map table names
            base.OnModelCreating(modelBuilder);
        }
    }
}
