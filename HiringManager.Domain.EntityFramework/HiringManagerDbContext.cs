﻿using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using HiringManager.EntityModel;

namespace HiringManager.EntityFramework
{
    public class HiringManagerDbContext : System.Data.Entity.DbContext, IDbContext
    {
        public IDbSet<Candidate> Candidates { get; set; }
        public IDbSet<CandidateStatus> CandidateStatuses { get; set; }
        public IDbSet<ContactInfo> ContactInfo { get; set; }
        public IDbSet<Manager> Managers { get; set; }
        public IDbSet<Position> Positions { get; set; }

        public IQueryable<T> Query<T>() where T: class
        {
            return base.Set<T>();
        }

        public void Add<T>(T item) where T:class
        {
            base.Set<T>().Add(item);
        }

        public void Delete<T>(T item) where T:class
        {
            base.Set<T>().Remove(item);
        }

        public new void SaveChanges()
        {
            try
            {
                base.SaveChanges();
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbeve)
            {
                Trace.Indent();

                foreach (var dbEntityValidationResult in dbeve.EntityValidationErrors)
                {
                    var typeName = dbEntityValidationResult.Entry.Entity.GetType().Name;
                    var message = string.Format("Error saving {0}; {1}", typeName, dbEntityValidationResult.Entry.State.ToString());
                    Trace.WriteLine(message);
                    foreach (var dbValidationError in dbEntityValidationResult.ValidationErrors)
                    {
                        var errorMessage = dbValidationError.PropertyName + "; " + dbValidationError.ErrorMessage;
                        Trace.WriteLine(errorMessage);
                    }
                }
                Trace.Flush();

                Trace.Unindent();
                throw;
            }
        }

        public T Get<T>(int key) where T:class
        {
            var result =base.Set<T>().Find(key);
            return result;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Candidate>()
                .HasMany(row => row.AppliedTo)
                .WithRequired(row => row.Candidate)
                ;
            modelBuilder.Entity<CandidateStatus>();
            modelBuilder.Entity<ContactInfo>();
            modelBuilder.Entity<Manager>()
                ;



            modelBuilder.Entity<Position>()
                .HasMany(row => row.Candidates)
                .WithRequired(row => row.Position)
                ;

            modelBuilder.Entity<Position>()
                .HasRequired(row => row.CreatedBy)
                .WithMany(row => row.Positions)
                ;

            modelBuilder.Entity<Position>()
                .HasMany(row => row.Openings)
                .WithRequired(row=> row.Position)
                ;

            modelBuilder.Entity<Opening>()
                .HasRequired(m => m.Position)
                ;

            modelBuilder.Entity<Opening>()
                .HasOptional(m => m.FilledBy)
                ;

        }

        public System.Data.Entity.DbSet<HiringManager.EntityModel.Source> Sources { get; set; }


    }
}