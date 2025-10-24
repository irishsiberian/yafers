using Microsoft.EntityFrameworkCore;
using Yafers.Web.Data.Entities;

namespace Yafers.Web.Data
{
    public class EntityConfigurator
    {
        public void Configure(ModelBuilder builder)
        {
            builder.Entity<Audit>(entity =>
            {
                entity.ToTable("Audits");
                entity.HasKey(e => e.Id);
            });

            builder.Entity<Competition>(entity =>
            {
                entity.ToTable("Competitions");
                entity.HasKey(e => e.Id);
                entity.HasMany(c => c.Rounds)
                    .WithOne(r => r.Competition)
                    .HasForeignKey(r => r.CompetitionId);
                entity.HasOne(c => c.Dance)
                    .WithMany() 
                    .HasForeignKey(c => c.DanceId);
            });

            builder.Entity<DancerRegistration>(entity =>
            {
                entity.ToTable("DancerRegistrations");
                entity.HasKey(e => e.Id);
                entity.HasMany(c => c.CompetitionRegistrations)
                    .WithOne(c => c.DancerRegistration)
                    .HasForeignKey(c => c.DancerRegistrationId);
            });

            builder.Entity<CompetitionRegistration>(entity =>
            {
                entity.ToTable("CompetitionRegistrations");
                entity.HasKey(e => e.Id);
                entity.HasOne(c => c.Dancer)
                    .WithMany()
                    .HasForeignKey(c => c.DancerId);
                entity.HasOne(c => c.Registrar)
                    .WithMany() 
                    .HasForeignKey(c => c.RegistrarId);
                entity.HasOne(c => c.Competition)
                    .WithMany()
                    .HasForeignKey(c => c.CompetitionId);
                entity.HasOne(c => c.Feis)
                    .WithMany(f => f.CompetitionRegistrations)
                    .HasForeignKey(c => c.FeisId);
                entity.HasOne(c => c.Invoice)
                    .WithMany(i => i.CompetitionRegistrations)
                    .HasForeignKey(c => c.InvoiceId);
                entity.HasOne(c => c.Team)
                    .WithMany()
                    .HasForeignKey(c => c.TeamId);
            });

            builder.Entity<Dancer>(entity =>
            {
                entity.ToTable("Dancers");
                entity.HasKey(e => e.Id);
                entity.HasOne(c => c.School)
                    .WithMany() 
                    .HasForeignKey(c => c.SchoolId);
                entity.HasOne(c => c.DancerParent)
                    .WithMany()
                    .HasForeignKey(c => c.DancerParentId);
                entity.HasOne(c => c.DancerParentUser)
                    .WithMany()
                    .HasForeignKey(c => c.DancerParentUserId);
                entity.HasMany(d => d.Registrations)
                    .WithOne()
                    .HasForeignKey(r => r.DancerId);
            });

            builder.Entity<DancerParent>(entity =>
            {
                entity.ToTable("DancerParents");
                entity.HasKey(e => e.Id);
                entity.HasOne(c => c.DancerParentUser)
                    .WithMany()
                    .HasForeignKey(c => c.UserId);
                entity.HasMany(c => c.Dancers)
                    .WithOne(d => d.DancerParent)
                    .HasForeignKey(d => d.DancerParentId);
            });

            builder.Entity<Feis>(entity =>
            {
                entity.ToTable("Feiseanna");
                entity.HasKey(e => e.Id);
                entity.HasOne(c => c.Syllabus)
                    .WithMany()
                    .HasForeignKey(c => c.SyllabusId);
                entity.HasOne(c => c.OrganiserSchool)
                    .WithMany()
                    .HasForeignKey(c => c.OrganiserSchoolId);
                entity.HasOne(c => c.Association)
                    .WithMany()
                    .HasForeignKey(c => c.AssociationId);
                entity.HasMany(c => c.Invoices)
                    .WithOne(d => d.Feis)
                    .HasForeignKey(d => d.FeisId);
                entity.HasMany(c => c.CompetitionRegistrations)
                    .WithOne(d => d.Feis)
                    .HasForeignKey(d => d.FeisId);
                entity.HasMany(c => c.DancerRegistrations)
                    .WithOne(d => d.Feis)
                    .HasForeignKey(d => d.FeisId);
                entity.HasMany(c => c.Reports)
                    .WithOne()
                    .HasForeignKey(d => d.FeisId);
            });

            builder.Entity<Organiser>(entity =>
            {
                entity.ToTable("Organisers");
                entity.HasKey(e => e.Id);
                entity.HasOne(c => c.OrganiserUser)
                    .WithMany()
                    .HasForeignKey(c => c.UserId);
            });

            builder.Entity<Teacher>(entity =>
            {
                entity.ToTable("Teachers");
                entity.HasKey(e => e.Id);
                entity.HasOne(c => c.TeacherUser)
                    .WithMany()
                    .HasForeignKey(c => c.UserId);
            });

            builder.Entity<School>(entity =>
            {
                entity.ToTable("Schools");
                entity.HasKey(e => e.Id);
                entity.HasOne(c => c.Parent)
                    .WithMany(c => c.Branches)
                    .HasForeignKey(c => c.ParentId);
                entity.HasMany(c => c.Teachers)
                    .WithOne()
                    .HasForeignKey(d => d.SchoolId);
            });

            builder.Entity<Syllabus>(entity =>
            {
                entity.ToTable("Syllabi");
                entity.HasKey(e => e.Id);
                entity.HasMany(s => s.Competitions)
                    .WithMany(c => c.Syllabi)
                    .UsingEntity<SyllabusCompetition>(
                        j => j
                            .HasOne(sc => sc.Competition)
                            .WithMany()
                            .HasForeignKey(sc => sc.CompetitionId),
                        j => j
                            .HasOne(sc => sc.Syllabus)
                            .WithMany()
                            .HasForeignKey(sc => sc.SyllabusId),
                        j =>
                        {
                            j.ToTable("SyllabusCompetitions");
                            j.HasKey(x => x.Id);
                        });
            });

            builder.Entity<SyllabusCompetition>(entity =>
            {
                entity.ToTable("SyllabusCompetitions");

                entity.HasKey(sc => sc.Id);

                entity.HasOne(sc => sc.Syllabus)
                    .WithMany(s => s.SyllabusCompetitions)
                    .HasForeignKey(sc => sc.SyllabusId);

                entity.HasOne(sc => sc.Competition)
                    .WithMany(c => c.SyllabusCompetitions)
                    .HasForeignKey(sc => sc.CompetitionId);
            });
        }
    }
}
