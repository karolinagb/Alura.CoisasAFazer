using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Xunit;
using Moq;

namespace Alura.CoisasAFazer.Testes
{
    public class CadastraTarefaHandlerExecute
    {
        //Cenário 1
        //arrange
        //Dado informações de tarefa
        //act
        //Salvar no banco de dados
        //assert
        [Fact]
        public void IncluiNoBdDadaTarefaValida()
        {
            //Para criar nossa tarefa
            //Criar comando
            //Executar o comando

            //arrange
            var comando = new CadastraTarefa("Estudar xUnit", new Categoria("Estudo"), new DateTime(2021, 12, 31));

            var options = new DbContextOptionsBuilder<DbTarefasContext>()
                .UseInMemoryDatabase("DbTarefas")
                .Options;
            var context = new DbTarefasContext(options);

            var repo = new RepositorioTarefa(context);

            //tratador do comando acima
            var handler = new CadastraTarefaHandler(repo);

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

            //Ao invés de usar o bd em memória, eu posso usar o mock
            var mock = new Mock<IRepositorioTarefas>();

            //Aqui faz as configurações como lançamento de exceção
            mock.Setup(obj => obj.IncluirTarefas(It.IsAny<Tarefa[]>()))//It.IsAny == qualquer array de tarefa recebido ele vai lançar exceção
                .Throws(new Exception("Houve um erro na inclusão de tarefas"));

            var repo = mock.Object; //Dá um objeto pra gente daquilo tipo passado na instancia do mock

            //tratador do comando acima
            var handler = new CadastraTarefaHandler(repo);

            //act
            CommandResult resultado = handler.Execute(comando); //System Under Test (SUT - Sistema sob teste) -> CadastraTarefaHandlerExecute

            //assert

            //Assert.True(true);
            Assert.False(resultado.IsSuccess); //Verifica se e false
        }
    }
}
