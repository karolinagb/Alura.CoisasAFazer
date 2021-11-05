using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
using Alura.CoisasAFazer.WebApp.Controllers;
using Alura.CoisasAFazer.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using Xunit;

namespace Alura.CoisasAFazer.Testes
{
    public class TarefasControllerEndpointCadastraTarefa
    {
        [Fact]
        public void DadoTarefaRetornar200()
        {
            //arrange
            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();
            var log = mockLogger.Object;

            var options = new DbContextOptionsBuilder<DbTarefasContext>()
                .UseInMemoryDatabase("DbTarefasRetorna200")
                .Options;
            var contexto = new DbTarefasContext(options);

            contexto.Categorias.Add(new Categoria(20, "Estudo"));
            contexto.SaveChanges();

            var repo = new RepositorioTarefa(contexto);

            var controlador = new TarefasController(repo, log);
            var model = new CadastraTarefaVM();
            model.IdCategoria = 20;
            model.Titulo = "Estudar xUnit";
            model.Prazo = new DateTime(2019, 12, 31);

            //act
            var retorno = controlador.EndpointCadastraTarefa(model);

            //assert
            Assert.IsType<OkResult>(retorno); //Http 200
        }

        [Fact]
        public void DadoExcecaoLancadaRetornar500()
        {
            //arrange
            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();
            var log = mockLogger.Object;

            //Preciso gerar uma exceção ao chamar o método incluir tarefa então preciso do stuby
            var mockRepo = new Mock<IRepositorioTarefas>();
            mockRepo.Setup(r => r.ObtemCategoriaPorId(20)).Returns(new Categoria(20, "Estudo"));
            mockRepo.Setup(r => r.IncluirTarefas(It.IsAny<Tarefa[]>())).Throws(new Exception("Houve um erro"));
            var repo = mockRepo.Object;

            var controlador = new TarefasController(repo, log);
            var model = new CadastraTarefaVM();
            model.IdCategoria = 20;
            model.Titulo = "Estudar xUnit";
            model.Prazo = new DateTime(2019, 12, 31);

            //act
            var retorno = controlador.EndpointCadastraTarefa(model);

            //assert
            Assert.IsType<StatusCodeResult>(retorno); //Http 400
            var statusCodeRetornado = (retorno as StatusCodeResult).StatusCode;
            Assert.Equal(500, statusCodeRetornado);
        }
    }
}