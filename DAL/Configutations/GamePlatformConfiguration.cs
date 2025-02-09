using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.Configutations;
public class GamePlatformConfiguration : IEntityTypeConfiguration<GamePlatform>
{
    public void Configure(EntityTypeBuilder<GamePlatform> builder)
    {
        builder.HasKey(b => new { b.GameId, b.PlatformId });

        builder
            .HasOne(b => b.Platform)
            .WithMany();

        builder
           .HasOne(b => b.Game)
           .WithMany();
    }
}
