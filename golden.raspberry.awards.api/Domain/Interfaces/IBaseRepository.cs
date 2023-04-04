using golden.raspberry.awards.api.Domain.Entities;
using System.Collections.Generic;

namespace golden.raspberry.awards.api.Domain.Interfaces
{
    public interface IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        /// <summary>
        /// Retorna a lista de todos os objetos do tipo da entidade solicitada.
        /// </summary>
        IList<TEntity> Select();

        /// <summary>
        /// Retorna um objeto do tipo da entidade solicitada através do seu identificador.
        /// </summary>
        /// <param name="id">Identificador do objeto solicitado.</param>
        TEntity Select(int id);

        /// <summary>
        /// Adiciona um objeto do tipo da entidade solicitada.
        /// </summary>
        /// <param name="obj">Objeto que será inserido.</param>
        void Insert(TEntity obj);

        /// <summary>
        /// Atualiza as propriedades de um objeto do tipo da entidade solicitada.
        /// </summary>
        /// <param name="obj">Objeto que será atualizado.</param>
        void Update(TEntity obj);

        /// <summary>
        /// Deleta um objeto do tipo da entidade solicitada através do seu identificador.
        /// </summary>
        /// <param name="id">Identificador do objeto a ser deletado.</param>
        void Delete(int id);
    }
}