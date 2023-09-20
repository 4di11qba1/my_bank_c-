protected override void OnModelCreating(ModelBuilder modelBuilder) {
    modelBuilder.Entity<Patient>
        .HasOne(d=>d.Doctors)
        .WithMany(p=>p.Patients)
        .HasForiegnKey(i=>i.DoctorsID)
        .OnDelete(DeleteBehavior.Restrict);
}