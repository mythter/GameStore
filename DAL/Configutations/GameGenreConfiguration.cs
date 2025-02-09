using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configutations;
public class GameGenreConfiguration : IEntityTypeConfiguration<GameGenre>
{
    public void Configure(EntityTypeBuilder<GameGenre> builder)
    {
        builder.HasKey(b => new { b.GameId, b.GenreId });

        builder
            .HasOne(b => b.Genre)
            .WithMany();

        builder
           .HasOne(b => b.Game)
           .WithMany();
    }
}
