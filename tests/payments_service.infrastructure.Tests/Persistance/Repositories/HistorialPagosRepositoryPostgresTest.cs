using Microsoft.EntityFrameworkCore;
using payments_services.domain.Entities;
using payments_services.infrastructure.Persistence.Context;
using payments_services.infrastructure.Persistence.Repositories;
using payments_services.infrastructure.Persistence.Models;
using Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace payments_services.tests.Infrastructure
{
    public class HistorialPagosRepositoryPostgresTests
    {
        private AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task RegistrarHistorialPagosAsync_ShouldPersistPayment()
        {
            var dbContext = CreateDbContext();
            var repository = new HistorialPagosRepositoryPostgres(dbContext);
            var historial = new HistorialPagos
            {
                IdUsuario = Guid.NewGuid(),
                IdEvento = Guid.NewGuid(),
                MontoPago = new MontoHistorialPagosVO(50),
                IdMedioDePago = "pm_123",
                UltimosDigitosTarjeta = "4242",
                TipoMedioDePago = "visa"
            };

            var resultId = await repository.RegistrarHistorialPagosAsync(historial);

            var persisted = await dbContext.HistorialPagos.FindAsync(resultId);
            Assert.NotNull(persisted);
            Assert.Equal(resultId, persisted.Id);
            Assert.Equal(50, persisted.Monto);
        }

        [Fact]
        public async Task GetHistorialDePagoByEvento_ShouldReturnCorrectDomainObject()
        {
            var dbContext = CreateDbContext();
            var repository = new HistorialPagosRepositoryPostgres(dbContext);
            var eventoId = Guid.NewGuid();

            await dbContext.HistorialPagos.AddAsync(new HistorialPagosPostgreSQL
            {
                Id = Guid.NewGuid(),
                IdEvento = eventoId,
                IdUsuario = Guid.NewGuid(),
                Monto = 100,
                IdMedioDePago = "pm_test",
                UltimosCuatroDigitos = "1234",
                TipoMedioDePago = "Mastercard",
                CreatedAt = DateTime.UtcNow
            });
            await dbContext.SaveChangesAsync();

            var result = await repository.GetHistorialDePagoByEvento(eventoId, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(eventoId, result.IdEvento);
            Assert.Equal("1234", result.UltimosDigitosTarjeta);
        }

        [Fact]
        public async Task GetHistorialPagosByUserAsync_ShouldFilterByUserId()
        {
            var dbContext = CreateDbContext();
            var repository = new HistorialPagosRepositoryPostgres(dbContext);
            var userId = Guid.NewGuid();

            await dbContext.HistorialPagos.AddRangeAsync(new List<HistorialPagosPostgreSQL>
            {
                new HistorialPagosPostgreSQL { Id = Guid.NewGuid(), IdUsuario = userId, IdEvento = Guid.NewGuid(), IdMedioDePago = "1", UltimosCuatroDigitos = "1", TipoMedioDePago = "1" },
                new HistorialPagosPostgreSQL { Id = Guid.NewGuid(), IdUsuario = userId, IdEvento = Guid.NewGuid(), IdMedioDePago = "2", UltimosCuatroDigitos = "2", TipoMedioDePago = "2" },
                new HistorialPagosPostgreSQL { Id = Guid.NewGuid(), IdUsuario = Guid.NewGuid(), IdEvento = Guid.NewGuid(), IdMedioDePago = "3", UltimosCuatroDigitos = "3", TipoMedioDePago = "3" }
            });
            await dbContext.SaveChangesAsync();

            var result = await repository.GetHistorialPagosByUserAsync(userId, CancellationToken.None);

            Assert.Equal(2, result.Count);
            Assert.All(result, r => Assert.Equal(userId, r.IdUsuario));
        }

        [Fact]
        public async Task ExistePago_ShouldReturnTrue_WhenRecordExists()
        {
            var dbContext = CreateDbContext();
            var repository = new HistorialPagosRepositoryPostgres(dbContext);
            var eventoId = Guid.NewGuid();

            await dbContext.HistorialPagos.AddAsync(new HistorialPagosPostgreSQL
            {
                Id = Guid.NewGuid(),
                IdEvento = eventoId,
                IdUsuario = Guid.NewGuid(),
                IdMedioDePago = "pm_1",
                UltimosCuatroDigitos = "1111",
                TipoMedioDePago = "visa",
                Monto = 10
            });
            await dbContext.SaveChangesAsync();

            var result = await repository.ExistePago(eventoId, CancellationToken.None);

            Assert.True(result);
        }

        [Fact]
        public async Task GetAllPagosAsync_ShouldReturnAllRecords()
        {
            var dbContext = CreateDbContext();
            var repository = new HistorialPagosRepositoryPostgres(dbContext);

            await dbContext.HistorialPagos.AddRangeAsync(new List<HistorialPagosPostgreSQL>
            {
                new HistorialPagosPostgreSQL { Id = Guid.NewGuid(), IdUsuario = Guid.NewGuid(), IdEvento = Guid.NewGuid(), IdMedioDePago = "1", UltimosCuatroDigitos = "1", TipoMedioDePago = "1" },
                new HistorialPagosPostgreSQL { Id = Guid.NewGuid(), IdUsuario = Guid.NewGuid(), IdEvento = Guid.NewGuid(), IdMedioDePago = "2", UltimosCuatroDigitos = "2", TipoMedioDePago = "2" }
            });
            await dbContext.SaveChangesAsync();

            var result = await repository.GetAllPagosAsync(CancellationToken.None);

            Assert.Equal(2, result.Count);
        }
    }
}