using Xunit;
using Moq;
using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Services.Handlers;
using Alura.CoisasAFazer.Infrastructure;

namespace Alura.CoisasAFazer.Testes
{
    public class ObtemCategoriaPorIdHandlerExecute
    {
        //O nome do metodo deve conter o cenario e o assert, já o nome da classe deve ser:
        //O nome da classe que contém o método sob teste + o nome do método sob teste

        //Dado Id da categoria
        //Quando acionar o método Execute
        //Retornar categoria por Id
        [Fact] 
        public void DadoIdCategoriaChamaObtemCategoriaPorIdUmaVez() 
        {
            //Arrange
            var idCategoria = 20;
            var commando = new ObtemCategoriaPorId(idCategoria);

            var mock = new Mock<IRepositorioTarefas>();
            var repo = mock.Object;

            var handler = new ObtemCategoriaPorIdHandler(repo);

            //Act
            handler.Execute(commando);

            //Assert
            mock.Verify(r => r.ObtemCategoriaPorId(idCategoria), Times.Once());
        }
    }
}
