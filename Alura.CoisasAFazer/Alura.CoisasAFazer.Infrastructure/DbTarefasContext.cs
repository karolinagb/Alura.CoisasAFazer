using Alura.CoisasAFazer.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Alura.CoisasAFazer.Infrastructure
{
    public class DbTarefasContext : DbContext
    {
        public DbTarefasContext(DbContextOptions options) : base(options)
        {
        }

        public DbTarefasContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //A configuração do banco fake não pode ficar aqui porque quando estiver em produção queremos usar o banco real

            //Caso haja a configuração padrão de um provider de bd aqui, mas o builder já vier configurado, como no caso do nosso banco fake,
            //então com esse código eu ignoro o uso do provider padrão.
            if (optionsBuilder.IsConfigured)
            {
                return;
            }
        }

        public DbSet<Tarefa> Tarefas { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
    }
}
