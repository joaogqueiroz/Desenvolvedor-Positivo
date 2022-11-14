using System.Threading.Tasks;
using JHipsterNet.Core.Pagination;
using ProcurandoApartamento.Domain.Services.Interfaces;
using ProcurandoApartamento.Domain.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

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
            var listOfEstabelecimentosUpper = listOfEstabelecimentos.Select(s => s.ToUpper()).ToArray();
            var result = await _apartamentoRepository.QueryHelper()
                .Filter(apartamento => apartamento.ApartamentoDisponivel == true && apartamento.EstabelecimentoExiste == true && listOfEstabelecimentosUpper.Contains(apartamento.Estabelecimento))
                .GetAllAsync();

            if (result != null && result.Any()) 
            {
                Apartamento aparamentoMatch = new();
                var quadra = 0;
                if (result.Count() >= 2)
                {
                   var  resultComDistinct = result.DistinctBy(x => x.Quadra).ToList();
                    if ((result.ToList().Count) != (resultComDistinct).Count)
                    { 
                        var listAparamentoMatch = result.GroupBy(x => x.Quadra).Where(apartamento => apartamento.Count() > 1).ToList();
                        var final = listAparamentoMatch.SelectMany(x => x);
                        quadra = final.Max(Apartamento => Apartamento.Quadra);
                        aparamentoMatch = (Apartamento)final.Find(x => x.Quadra == quadra);
                        return aparamentoMatch;
                    }
                    quadra = result.Max(Apartamento => Apartamento.Quadra);
                    aparamentoMatch = (Apartamento)result.Find(x => x.Quadra == quadra);
                    return aparamentoMatch;
                }
                aparamentoMatch = (result.ToArray()).First();
                return aparamentoMatch;
            }
            Apartamento apartamentoNaoEncontrado = new();
            apartamentoNaoEncontrado.Estabelecimento = "Not found";
            return apartamentoNaoEncontrado;
        }

    }
}
