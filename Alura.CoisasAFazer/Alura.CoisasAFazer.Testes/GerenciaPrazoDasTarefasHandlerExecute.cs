using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Alura.CoisasAFazer.Testes
{
    public class GerenciaPrazoDasTarefasHandlerExecute
    {
        [Fact]
        public void MudaStatusDadoTarefasAtrasadas()
        {
            //arrange
            var compCateg = new Categoria(1, "Compras");
            var casaCateg = new Categoria(2, "Casa");
            var trabCateg = new Categoria(3, "Trabalho");
            var saudCateg = new Categoria(4, "Saúde");
            var higiCateg = new Categoria(5, "Higiene");

            var tarefas = new List<Tarefa>
            {
                //atrasadas a partir de 1/1/2021
                new Tarefa(1, "Tirar lixo", casaCateg, new DateTime(2018,12,31), null, StatusTarefa.Criada),
                new Tarefa(4, "Fazer o almoço", casaCateg, new DateTime(2017,12,1), null, StatusTarefa.Criada),
                new Tarefa(9, "Ir à academia", saudCateg, new DateTime(2018,12,31), null, StatusTarefa.Criada),
                new Tarefa(7, "Concluir o relatório", trabCateg, new DateTime(2018,5,7), null, StatusTarefa.Pendente),
                new Tarefa(10, "Beber água", saudCateg, new DateTime(2018,12,31), null, StatusTarefa.Criada),
                //dentro do prazo em 1/1/2021
                new Tarefa(8, "Comparecer à reunião", trabCateg, new DateTime(2019,11,12), new DateTime(2018,11,30), StatusTarefa.Concluida),
                new Tarefa(2, "Arrumar a cama", casaCateg, new DateTime(2021,4,5), null, StatusTarefa.Criada),
                new Tarefa(3, "Escovar os dentes", higiCateg, new DateTime(2021,1,2), null, StatusTarefa.Criada),
                new Tarefa(5, "Comprar presente pro João", compCateg, new DateTime(2021,10,8), null, StatusTarefa.Criada),
                new Tarefa(6, "Comprar ração", compCateg, new DateTime(2021,11,20), null, StatusTarefa.Criada),
            };

            //Definindo configurações do provedor
            var options = new DbContextOptionsBuilder<DbTarefasContext>()
                .UseInMemoryDatabase("DbTarefasEmAtraso")
                .Options;

            //Instanciando classe de contexto (representa o EF) passando o provedor
            var contexto = new DbTarefasContext(options);

           //Instanciando o repositório passando o contexto
            var repo = new RepositorioTarefa(contexto);

            repo.IncluirTarefas(tarefas.ToArray());

            var comando = new GerenciaPrazoDasTarefas(new DateTime(2021,1,1));
            var handler = new GerenciaPrazoDasTarefasHandler(repo);

            //act
            handler.Execute(comando);

            //assert
            var tarefasEmAtraso = repo.ObtemTarefas(t => t.Status == StatusTarefa.EmAtraso);
            Assert.Equal(5, tarefasEmAtraso.Count());
        }

        //Dado 3 tarefas e chamada do método ObtemTarefas
        //Quando chamado metodo Execute
        //Atualize tarefas deve ser chamado 3 vezes (quantidade de tarefas adicionadas)
        [Fact]
        public void DadoExecuteInvocadoChamarAtualizarTarefaNTarefas()
        {
            //arrange
            var tarefas = new List<Tarefa>
            {
                new Tarefa(1, "Tirar lixo", new Categoria("Casa"), new DateTime(2018,12,31), null, StatusTarefa.Criada),
                new Tarefa(4, "Fazer o almoço",new Categoria("Casa"), new DateTime(2017,12,1), null, StatusTarefa.Criada),
                new Tarefa(9, "Ir à academia", new Categoria("Saúde"), new DateTime(2018,12,31), null, StatusTarefa.Criada)
            };
            var mock = new Mock<IRepositorioTarefas>();
            mock.Setup(obj => obj.ObtemTarefas(It.IsAny<Func<Tarefa, bool>>())).Returns(tarefas);
            var repo = mock.Object;

            var commando = new GerenciaPrazoDasTarefas(new DateTime(2019, 1, 1));
            var handler = new GerenciaPrazoDasTarefasHandler(repo);

            //Act
            handler.Execute(commando);

            //Assert 
            //Par qualquer argumento passado ao metodo AtualizarTarefas verificar se foi chamado 3 vezes
            mock.Verify(obj => obj.AtualizarTarefas(It.IsAny<Tarefa[]>()), Times.Once()); 
            //O Verify faz algo parecido com o Equal, aqui ele verifica se o metodo Atualizar tarefas é chamado 1 vez
        }
    }
}
