using System.Threading.Tasks;
using JHipsterNet.Core.Pagination;
using ProcurandoApartamento.Domain.Services.Interfaces;
using ProcurandoApartamento.Domain.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ProcurandoApartamento.Domain.Services
{
    public class ApartamentoService : IApartamentoService
    {
        protected readonly IApartamentoRepository _apartamentoRepository;

        public ApartamentoService(IApartamentoRepository apartamentoRepository)
        {
            _apartamentoRepository = apartamentoRepository;
        }

        public virtual async Task<Apartamento> Save(Apartamento apartamento)
        {
            await _apartamentoRepository.CreateOrUpdateAsync(apartamento);
            await _apartamentoRepository.SaveChangesAsync();
            return apartamento;
        }

        public virtual async Task<IPage<Apartamento>> FindAll(IPageable pageable)
        {
            var page = await _apartamentoRepository.QueryHelper()
                .GetPageAsync(pageable);
            return page;
        }

        public virtual async Task<Apartamento> FindOne(long id)
        {
            var result = await _apartamentoRepository.QueryHelper()
                .GetOneAsync(apartamento => apartamento.Id == id);
            return result;
        }

        public virtual async Task Delete(long id)
        {
            await _apartamentoRepository.DeleteByIdAsync(id);
            await _apartamentoRepository.SaveChangesAsync();
        }

        public async Task<Apartamento> FindMelhorApartamento(string[] listOfEstabelecimentos)
        {
            var result = await _apartamentoRepository.QueryHelper()
                .Filter(Apartamento => Apartamento.ApartamentoDisponivel == true && Apartamento.EstabelecimentoExiste == true && Apartamento.Estabelecimento == listOfEstabelecimentos[0]).GetAllAsync();

            if (result != null)
            {
                Apartamento aparamentoMatch = new();

                if (result.Count() >= 2)
                {
                    var quadra = result.Max(Apartamento => Apartamento.Quadra);
                    aparamentoMatch = (Apartamento)result.Find(x => x.Quadra == quadra);

                    return aparamentoMatch;
                }
                //aparamentoMatch = result.First();
                return result.First();

            }
            Apartamento teste = new();
            return teste;
        }

    }
}
