using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Web.UI.WebControls;

namespace KaiutYoga.Models
{
    public class KaiutYogaContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, add the following
        // code to the Application_Start method in your Global.asax file.
        // Note: this will destroy and re-create your database with every model change.
        // 
        //System.Data.Entity.Database.SetInitializer(new System.Data.Entity.DropCreateDatabaseIfModelChanges<KaiutYoga.Context.KaiutYogaContext>());
        public KaiutYogaContext() : base("name=KaiutYogaBoqueirao")
        {
            
        }


        public DbSet<RoomModel> RoomModels { get; set; }
        public DbSet<ProfessorModel> ProfessorModels { get; set; }
        public DbSet<ClassModel> ClassModels { get; set; }
        public DbSet<PlanModel> PlanModels { get; set; }
        public DbSet<StudentModel> StudentModels { get; set; }
        public DbSet<PresenceModel> PresenceModels { get; set; }
        public DbSet<SpareClassModel> SpareClassModels { get; set; }
        public DbSet<PaymentModel> PaymentModels { get; set; }
        //public DbSet<CreateClassModel> CreateClassModels { get; set; }

        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<IdentityUserLogin>().HasKey<string>(l => l.UserId);
            //modelBuilder.Entity<IdentityRole>().HasKey<string>(r => r.Id);
            //modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId });


            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<PresenceModel>().HasMany(c => c.EnroledWeeklyStudents).WithMany(c=>c.EnroledWeeklyClasses);
            modelBuilder.Entity<PresenceModel>().HasMany(c => c.PresentWeeklyStudents).WithMany(c => c.WeeklyPresence);
            modelBuilder.Entity<PresenceModel>().HasMany(c => c.EnroledTrialStudents).WithMany(c => c.EnroledTrialClasses);
            modelBuilder.Entity<PresenceModel>().HasMany(c => c.PresentTrialStudents).WithMany(c => c.TrialPresence);
            modelBuilder.Entity<PresenceModel>().HasMany(c => c.EnroledReplacementStudents).WithMany(c => c.EnroledReplacementClasses);
            modelBuilder.Entity<PresenceModel>().HasMany(c => c.PresentReplacementStudents).WithMany(c => c.ReplacementPresence);
        }

        public class KaiutYogaContextInitializer : DropCreateDatabaseIfModelChanges<KaiutYogaContext>
        //public class KaiutYogaContextInitializer : DropCreateDatabaseAlways<KaiutYogaContext>
        {
            protected override void Seed(KaiutYogaContext context)
            {
                /*
                this.Professor(context);
                this.Room(context);
                this.Plans(context);
                
                this.Classes(context, DayOfWeek.Wednesday);
                this.Classes(context, DayOfWeek.Monday);
                this.Classes(context, DayOfWeek.Friday);
                this.Classes(context, DayOfWeek.Tuesday);
                this.Classes(context, DayOfWeek.Thursday);
                this.Students(context);
                
                base.Seed(context);
                */
            }

            private void Professor(KaiutYogaContext context)
            {
                ProfessorModel pm = new ProfessorModel();
                pm.Name = "Solange";
                context.ProfessorModels.Add(pm);
                context.SaveChanges();
            }

            private void Room(KaiutYogaContext context)
            {
                RoomModel rm = new RoomModel();
                rm.Name = "Sala";
                rm.Capacity = 20;
                rm.SpareCapacity = 3;
                rm.Location = "-";
                context.RoomModels.Add(rm);
                context.SaveChanges();
            }

            private void Plans(KaiutYogaContext context)
            {
                PlanModel pm;

                pm = new PlanModel();
                pm.Name = "1x semana";
                pm.Frequency = 1;
                pm.IsTrial = false;
                pm.Price = 140;
                pm.LastDayToPayWithDiscount = 10;
                context.PlanModels.Add(pm);
                context.SaveChanges();

                pm = new PlanModel();
                pm.Name = "2x semana";
                pm.Frequency = 2;
                pm.IsTrial = false;
                pm.Price = 180;
                pm.LastDayToPayWithDiscount = 10;
                context.PlanModels.Add(pm);
                context.SaveChanges();

                pm = new PlanModel();
                pm.Name = "3x semana";
                pm.Frequency = 3;
                pm.IsTrial = false;
                pm.Price = 220;
                pm.LastDayToPayWithDiscount = 10;
                context.PlanModels.Add(pm);
                context.SaveChanges();

                pm = new PlanModel();
                pm.Name = "Experimental";
                pm.Frequency = 4;
                pm.IsTrial = true;
                pm.Price = 0;
                //pm.LastDayToPayWithDiscount = ;
                context.PlanModels.Add(pm);
                context.SaveChanges();
            }

            private void Classes (KaiutYogaContext context, DayOfWeek day)
            {
                List<ClassModel> classes = new List<ClassModel>();
                ClassModel aula;
                DateTime start = DateTime.ParseExact("07:30", "HH:mm", null);
                DateTime end = DateTime.ParseExact("19:00", "HH:mm", null);
                for (DateTime i = start; i <= end; i = i.AddMinutes(75))
                {
                    aula = new ClassModel();
                    aula.Day = day;
                    aula.ProfessorId = 1;
                    aula.RoomId = 1;
                    aula.SelectedTime = i.ToString("HH:mm");
                    classes.Add(aula);
                }

                foreach (ClassModel classmodel in classes)
                {
                    context.ClassModels.Add(classmodel);
                    context.SaveChanges();
                }
            }

            private void Students (KaiutYogaContext context)
            {
                StudentModel sm = new StudentModel();
                sm.FirstName = "Yuri";
                sm.LastName = "Pechnicki";
                sm.Observations = "Test";
                sm.BirthDate = DateTime.Now;
                //sm.ChangedPlan = DateTime.Now;
                sm.RegisterDate = DateTime.Now;
                sm.PlanId = 1;
                sm.Classes = new List<ClassModel>();
                sm.Classes.Add(context.ClassModels.Find(1));
                sm.Classes.Add(context.ClassModels.Find(2));
                sm.Classes.Add(context.ClassModels.Find(3));

                context.StudentModels.Add(sm);
                context.SaveChanges();
            }

        }
        
    }
}
