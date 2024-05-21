using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using PokemonsWebAPI.Models;
using Type = PokemonsWebAPI.Models.Type;

namespace PokemonsWebAPI.Context;

public partial class MgssswnzContext : DbContext
{
    public MgssswnzContext()
    {
    }

    public MgssswnzContext(DbContextOptions<MgssswnzContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Ability> Abilities { get; set; }

    public virtual DbSet<Activity> Activities { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<Pokemon> Pokemons { get; set; }

    public virtual DbSet<PokemonAbility> PokemonAbilities { get; set; }

    public virtual DbSet<PokemonActual> PokemonActuals { get; set; }

    public virtual DbSet<PokemonHarakteristik> PokemonHarakteristiks { get; set; }

    public virtual DbSet<PokemonParameter> PokemonParameters { get; set; }

    public virtual DbSet<PokemonType> PokemonTypes { get; set; }

    public virtual DbSet<Raiting> Raitings { get; set; }

    public virtual DbSet<Type> Types { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserActivity> UserActivities { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Server=mel.db.elephantsql.com;Database=mgssswnz;Username=mgssswnz;Password=BsZVLyhRmBjtdVhRas3RG0xVk3Do3SXc");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresExtension("btree_gin")
            .HasPostgresExtension("btree_gist")
            .HasPostgresExtension("citext")
            .HasPostgresExtension("cube")
            .HasPostgresExtension("dblink")
            .HasPostgresExtension("dict_int")
            .HasPostgresExtension("dict_xsyn")
            .HasPostgresExtension("earthdistance")
            .HasPostgresExtension("fuzzystrmatch")
            .HasPostgresExtension("hstore")
            .HasPostgresExtension("intarray")
            .HasPostgresExtension("ltree")
            .HasPostgresExtension("pg_stat_statements")
            .HasPostgresExtension("pg_trgm")
            .HasPostgresExtension("pgcrypto")
            .HasPostgresExtension("pgrowlocks")
            .HasPostgresExtension("pgstattuple")
            .HasPostgresExtension("tablefunc")
            .HasPostgresExtension("unaccent")
            .HasPostgresExtension("uuid-ossp")
            .HasPostgresExtension("xml2");

        modelBuilder.Entity<Ability>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Ability_pkey");

            entity.ToTable("Ability");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasMaxLength(200);
        });

        modelBuilder.Entity<Activity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Activity_pkey");

            entity.ToTable("Activity");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasMaxLength(200);
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Group_pkey");

            entity.ToTable("Group");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasMaxLength(255);
        });

        modelBuilder.Entity<Pokemon>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Pokemon_pkey");

            entity.ToTable("Pokemon");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.HarakteristikId).HasColumnName("Harakteristik_id");
            entity.Property(e => e.ParameterId).HasColumnName("Parameter_id");
            entity.Property(e => e.PhotoPath)
                .HasMaxLength(200)
                .HasColumnName("Photo_path");
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasOne(d => d.Harakteristik).WithMany(p => p.Pokemons)
                .HasForeignKey(d => d.HarakteristikId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pok_fk1");

            entity.HasOne(d => d.Parameter).WithMany(p => p.Pokemons)
                .HasForeignKey(d => d.ParameterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("pok_fk0");
        });

        modelBuilder.Entity<PokemonAbility>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Pokemon_Ability_pkey");

            entity.ToTable("Pokemon_Ability");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AbilityId).HasColumnName("Ability_id");
            entity.Property(e => e.PokemonId).HasColumnName("Pokemon_id");

            entity.HasOne(d => d.Ability).WithMany(p => p.PokemonAbilities)
                .HasForeignKey(d => d.AbilityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PokemonAbility_fk0");

            entity.HasOne(d => d.Pokemon).WithMany(p => p.PokemonAbilities)
                .HasForeignKey(d => d.PokemonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("Pokemonid_fk1");
        });

        modelBuilder.Entity<PokemonActual>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Pokemon_actual_pkey");

            entity.ToTable("Pokemon_actual");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Value).HasColumnType("character varying");

            entity.HasOne(d => d.Pokemon).WithMany(p => p.PokemonActuals)
                .HasForeignKey(d => d.PokemonId)
                .HasConstraintName("pokactual_fk0");
        });

        modelBuilder.Entity<PokemonHarakteristik>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Pokemon_harakteristiks_pkey");

            entity.ToTable("Pokemon_harakteristiks");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.SpecAttack).HasColumnName("Spec_attack");
            entity.Property(e => e.SpecProtect).HasColumnName("Spec_protect");
        });

        modelBuilder.Entity<PokemonParameter>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Pokemon_parameters_pkey");

            entity.ToTable("Pokemon_parameters");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AllInGame).HasColumnName("All_in_game");
            entity.Property(e => e.EvolutionStage).HasColumnName("Evolution_stage");
            entity.Property(e => e.ExpGroup).HasColumnName("Exp_group");
            entity.Property(e => e.HatchingTime).HasColumnName("Hatching_time");

            entity.HasOne(d => d.ExpGroupNavigation).WithMany(p => p.PokemonParameters)
                .HasForeignKey(d => d.ExpGroup)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("group_fk0");
        });

        modelBuilder.Entity<PokemonType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Pokemon_type_pkey");

            entity.ToTable("Pokemon_type");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.PokemonId).HasColumnName("Pokemon_id");
            entity.Property(e => e.TypeId).HasColumnName("Type_id");

            entity.HasOne(d => d.Pokemon).WithMany(p => p.PokemonTypes)
                .HasForeignKey(d => d.PokemonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PokemonType_fk1");

            entity.HasOne(d => d.Type).WithMany(p => p.PokemonTypes)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PokemonType_fk0");
        });

        modelBuilder.Entity<Raiting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Rating_pkey");

            entity.ToTable("Raiting");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('\"Rating_id_seq\"'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.DataSet).HasColumnName("Data_set");
            entity.Property(e => e.PokemonId).HasColumnName("Pokemon_id");
            entity.Property(e => e.UserId).HasColumnName("User_id");

            entity.HasOne(d => d.Pokemon).WithMany(p => p.Raitings)
                .HasForeignKey(d => d.PokemonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("PokemonId_fk0");

            entity.HasOne(d => d.User).WithMany(p => p.Raitings)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("UserId_fk0");
        });

        modelBuilder.Entity<Type>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Type_pkey");

            entity.ToTable("Type");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasMaxLength(200);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("User_pkey");

            entity.ToTable("User");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Password).HasMaxLength(400);
            entity.Property(e => e.PasswordChange).HasColumnName("Password_change");
            entity.Property(e => e.RoleId).HasColumnName("Role_id");
            entity.Property(e => e.Username).HasMaxLength(200);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_role_fk0");
        });

        modelBuilder.Entity<UserActivity>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("User_activity_pkey");

            entity.ToTable("User_activity");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ActivityId).HasColumnName("Activity_id");
            entity.Property(e => e.Date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.UserId).HasColumnName("User_id");

            entity.HasOne(d => d.Activity).WithMany(p => p.UserActivities)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("ActivityId_fk0");

            entity.HasOne(d => d.UserNavigation).WithMany(p => p.UserActivities)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("UserId_fk0");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("User_role_pkey");

            entity.ToTable("User_role");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Title).HasMaxLength(200);
        });
        modelBuilder.HasSequence("actual_seq");
        modelBuilder.HasSequence("group_id_seq");
        modelBuilder.HasSequence("POKEMON_ID_SEQ");
        modelBuilder.HasSequence("POKEMONEVOLUTION_ID_SEQ");
        modelBuilder.HasSequence("pokemonpower_id_seq");
        modelBuilder.HasSequence("pokemontype_id_seq");
        modelBuilder.HasSequence("power_id_seq");
        modelBuilder.HasSequence("rating_seq");
        modelBuilder.HasSequence("type_seq");
        modelBuilder.HasSequence("type_user_activity_seq");
        modelBuilder.HasSequence("user_activity_seq");
        modelBuilder.HasSequence("user_seq");
        modelBuilder.HasSequence("userrole_seq");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
