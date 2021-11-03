using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;
using Moq;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;

namespace Alura.CoisasAFazer.Testes
{
    public class CadastraTarefaHandlerExecute
    {
        //Cen�rio 1
        //arrange
        //Dado informa��es de tarefa
        //act
        //Salvar no banco de dados
        //assert
        [Fact]
        public void IncluiNoBdDadaTarefaValida()
        {
            //Para criar nova tarefa
            //Criar comando
            //Executar o comando

            //arrange
            var comando = new CadastraTarefa("Estudar xUnit", new Categoria("Estudo"), new DateTime(2021, 12, 31));

            //Usar o mock para passar uma instancia de ILogger
            var mock = new Mock<ILogger<CadastraTarefaHandler>>();
            var log = mock.Object;

            var options = new DbContextOptionsBuilder<DbTarefasContext>()
                .UseInMemoryDatabase("DbTarefas")
                .Options;
            var context = new DbTarefasContext(options);

            var repo = new RepositorioTarefa(context);

            //tratador do comando acima
            var handler = new CadastraTarefaHandler(repo, log);

            //act
            handler.Execute(comando); //System Under Test (SUT - Sistema sob teste) -> CadastraTarefaHandlerExecute

            //assert
            var tarefa = repo.ObtemTarefas(t => t.Titulo == "Estudar xUnit").FirstOrDefault();
            //Assert.True(true);
            Assert.NotNull(tarefa);
        }

        [Fact]
        public void DadoExeptionLancadaIsSuccessDeveSerFalse()
        {
            //arrange
            var comando = new CadastraTarefa("Estudar xUnit", new Categoria("Estudo"), new DateTime(2021, 12, 31));

            //Ao inv�s de usar o bd em mem�ria, eu posso usar o mock
            var mock = new Mock<IRepositorioTarefas>();

            //Simula um repositorio que lan�a uma excecao no metodo incluirtarefas
            //Aqui faz as configura��es como lan�amento de exce��o
            mock.Setup(obj => obj.IncluirTarefas(It.IsAny<Tarefa[]>()))//It.IsAny == qualquer array de tarefa recebido ele vai lan�ar exce��o
                .Throws(new Exception("Houve um erro na inclus�o de tarefas"));

            //Instancia do tipo simulado
            var repo = mock.Object; //D� um objeto pra gente daquilo tipo passado na instancia do mock

            //Usar o mock para passar uma instancia de ILogger
            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();
            var log = mockLogger.Object;

            //tratador do comando acima
            var handler = new CadastraTarefaHandler(repo, log);

            //act
            CommandResult resultado = handler.Execute(comando); //System Under Test (SUT - Sistema sob teste) -> CadastraTarefaHandlerExecute

            //assert

            //Assert.True(true);
            Assert.False(resultado.IsSuccess); //Verifica se e false
        }

        //Dado exce��o lancada
        //Quando chamar m�todo execute
        //Logar mensagem da exce��o

        [Fact]
        public void DadoExceptionLancadaLogarMensagemExecao()
        {
            //arrange
            var mensagemErro = "Houve um erro na inclus�o de tarefas";

            var excecao = new Exception(mensagemErro);

            var comando = new CadastraTarefa("Estudar xUnit", new Categoria("Estudo"), new DateTime(2021, 12, 31));

            //Ao inv�s de usar o bd em mem�ria, eu posso usar o mock
            var mock = new Mock<IRepositorioTarefas>();

            //Simula um repositorio que lan�a uma excecao no metodo incluirtarefas
            //Aqui faz as configura��es como lan�amento de exce��o
            mock.Setup(obj => obj.IncluirTarefas(It.IsAny<Tarefa[]>()))//It.IsAny == qualquer array de tarefa recebido ele vai lan�ar exce��o
                .Throws(excecao);

            //Instancia do tipo simulado
            var repo = mock.Object; //D� um objeto pra gente daquilo tipo passado na instancia do mock

            //Usar o mock para passar uma instancia de ILogger
            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();
            var log = mockLogger.Object;

            //tratador do comando acima
            var handler = new CadastraTarefaHandler(repo, log);

            //act
            CommandResult resultado = handler.Execute(comando);

            //assert
            //Log error e um metodo de extensao e nao da para fazer verifica��es ou setups com moq usando metodos de extensao
            //Pra isso vamos ter que usar o metodo mesmo que seria o Log, n�o o de extensao
            mockLogger.Verify(log => 
            log.Log(
                    LogLevel.Error, //N�vel de log (todos os niveis tem metodo que falicitam mas o mock nao aceita) => LogError
                    mensagemErro, //Id do Evento
                    It.IsAny<EventId>(), //Id do Evento
                    It.IsAny<Object>(), //Objeto que ser� logado, no caso seria a excecao, mas j� estou pegando ela em baixo
                    excecao, //exceca que sera logada
                    It.IsAny<Func<object, Exception, string>>()//Funcao Converte objeto e excecao numa string
                ), 
                Times.Once());
        }

        delegate void CapturaMensagemLog(LogLevel level, EventId eventId, object state, Exception exception, Func<object, Exception, string> function);

        [Fact]
        public void IncluiNoBdDadaTarefa()
        {
            //arrange
            var tituloTarefaEsperado = "Usar Moq para aprofundar conhecimento de API";
            var comando = new CadastraTarefa(tituloTarefaEsperado, new Categoria("Estudo"), new DateTime(2021, 12, 31));

            //Usar o mock para passar uma instancia de ILogger
            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();

            LogLevel levelCapturado = LogLevel.Debug;
            string mensagemCapturada = tituloTarefaEsperado;

            CapturaMensagemLog captura = (level, eventId, state, exception, func) =>
            {
                levelCapturado = level;
                mensagemCapturada = func(state, exception);
            };

            //Quero capturar todas as vezes que esse m�todo for chamado
            mockLogger.Setup(l => 
            l.Log(
                    It.IsAny<LogLevel>(),
                    It.IsAny<EventId>(), 
                    It.IsAny<Object>(), 
                    It.IsAny<Exception>(), 
                    It.IsAny<Func<object, Exception, string>>()
                )).Callback(captura);

            var log = mockLogger.Object;

            var mockRepo = new Mock<IRepositorioTarefas>();
            var repo = mockRepo.Object;

            //tratador do comando acima
            var handler = new CadastraTarefaHandler(repo, log);

            //Act
            handler.Execute(comando);

            //Assert
            Assert.Equal(LogLevel.Debug, levelCapturado);
            Assert.Contains(tituloTarefaEsperado, mensagemCapturada); //verificar se cont�m sub string dentro de string
        }
    }
}
