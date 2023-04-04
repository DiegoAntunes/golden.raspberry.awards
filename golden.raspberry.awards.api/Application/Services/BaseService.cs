using FluentValidation;
using golden.raspberry.awards.api.Domain.Entities;
using golden.raspberry.awards.api.Domain.Interfaces;
using System;
using System.Collections.Generic;

namespace golden.raspberry.awards.api.Application.Services
{
    public interface IBaseService<TEntity> where TEntity : BaseEntity
    {
        /// <summary>
        /// Retorna a lista de todos os objetos do tipo da entidade solicitada.
        /// </summary>
        IList<TEntity> Get();

        /// <summary>
        /// Retorna um objeto do tipo da entidade solicitada através do seu identificador.
        /// </summary>
        /// <param name="id">Identificador do objeto solicitado.</param>
        TEntity GetById(int id);

        /// <summary>
        /// Adiciona um objeto do tipo da entidade solicitada.
        /// </summary>
        /// <typeparam name="TValidator">Executa as validações específicas do tipo da entidade solicitada.</typeparam>
        /// <param name="obj">Objeto que será inserido.</param>
        /// <returns>Retorna o identificador do objeto inserido.</returns>
        TEntity Add<TValidator>(TEntity obj) where TValidator : AbstractValidator<TEntity>;

        /// <summary>
        /// Atualiza as propriedades de um objeto do tipo da entidade solicitada.
        /// </summary>
        /// <typeparam name="TValidator">Executa as validações específicas do tipo da entidade solicitada.</typeparam>
        /// <param name="obj">Objeto que será atualizado.</param>
        /// <returns>Retorna o objeto com as propriedades atualizadas.</returns>
        TEntity Update<TValidator>(TEntity obj) where TValidator : AbstractValidator<TEntity>;

        /// <summary>
        /// Deleta um objeto do tipo da entidade solicitada através do seu identificador.
        /// </summary>
        /// <param name="id">Identificador do objeto a ser deletado.</param>
        void Delete(int id);
    }


    public class BaseService<TEntity> : IBaseService<TEntity> where TEntity : BaseEntity
    {
        private readonly IBaseRepository<TEntity> _baseRepository;

        public BaseService(IBaseRepository<TEntity> baseRepository)
        {
            _baseRepository = baseRepository;
        }

        public TEntity Add<TValidator>(TEntity obj) where TValidator : AbstractValidator<TEntity>
        {
            Validate(obj, Activator.CreateInstance<TValidator>());
            _baseRepository.Insert(obj);
            return obj;
        }

        public void Delete(int id) => _baseRepository.Delete(id);

        public IList<TEntity> Get() => _baseRepository.Select();

        public TEntity GetById(int id) => _baseRepository.Select(id);

        public TEntity Update<TValidator>(TEntity obj) where TValidator : AbstractValidator<TEntity>
        {
            Validate(obj, Activator.CreateInstance<TValidator>());
            _baseRepository.Update(obj);
            return obj;
        }

        private void Validate(TEntity obj, AbstractValidator<TEntity> validator)
        {
            if (obj == null)
                throw new Exception("Register not found!");

            validator.ValidateAndThrow(obj);
        }
    }
}