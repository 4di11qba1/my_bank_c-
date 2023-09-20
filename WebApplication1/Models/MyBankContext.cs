using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models;

public partial class MyBankContext : DbContext
{
    public MyBankContext()
    {
    }

    public MyBankContext(DbContextOptions<MyBankContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Beneficiary> Beneficiaries { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {

        ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=MyBank;User Id=sa;Password=reallyStrongPwd123;TrustServerCertificate=true");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tmp_ms_x__3214EC271EB8D878");

            entity.ToTable("Account");

            entity.Property(e => e.Id)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("ID");
            entity.Property(e => e.Balance)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("balance");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Type)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.User).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Account_User");
        });

        modelBuilder.Entity<Beneficiary>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Benefici__3214EC27388EF760");

            entity.ToTable("Beneficiary");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AccountId)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("accountID");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.Account).WithMany(p => p.Beneficiaries)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_Beneficiary_Account");

            entity.HasOne(d => d.User).WithMany(p => p.Beneficiaries)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Beneficiary_User");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Transact__3214EC273C915738");

            entity.ToTable("Transaction");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AccountId)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("accountID");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(18, 5)")
                .HasColumnName("amount");
            entity.Property(e => e.RecieverId)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("recieverID");
            entity.Property(e => e.SenderId)
                .HasColumnType("decimal(18, 0)")
                .HasColumnName("senderID");
            entity.Property(e => e.TimeStamp)
                .HasColumnType("datetime")
                .HasColumnName("timeStamp");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("type");

            entity.HasOne(d => d.Account).WithMany(p => p.TransactionAccounts)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_Transaction_Account");

            entity.HasOne(d => d.Reciever).WithMany(p => p.TransactionRecievers)
                .HasForeignKey(d => d.RecieverId)
                .HasConstraintName("FK_Transaction_RecieverAccount");

            entity.HasOne(d => d.Sender).WithMany(p => p.TransactionSenders)
                .HasForeignKey(d => d.SenderId)
                .HasConstraintName("FK_Transaction_SenderAccount");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC075A7055F3");

            entity.ToTable("User");

            entity.Property(e => e.Address)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("city");
            entity.Property(e => e.Cnic)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("CNIC");
            entity.Property(e => e.Email)
                .HasMaxLength(25)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
