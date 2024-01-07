using AcePacific.Common.Contract;
using AcePacific.Data.Entities;
using AcePacific.Data.Repositories;
using AcePacific.Data.ViewModel;
using AutoMapper;
using static AcePacific.Data.ViewModel.BankFilter;

namespace AcePacific.Busines.Services
{
    public interface IBankService
    {
        Task<Response<CountModel<BankItem>>> GetCount(int page, int pagesize, BankFilter filter = null);
        Task<IEnumerable<BankItem>> Query(int page, int pagesize, BankFilter filter);
        Task<Response<BankModel>> CreateBank(CreateBank model);
        Task<Response<BankModel>> GetBankById(int id);
        Task<Response<IEnumerable<BankModel>>> BankList();
    }
    public class BankService : IBankService
    {
        private readonly IMapper _mapper;
        private readonly IBankRepository _bankRepository;
        public BankService(IMapper mapper, IBankRepository bankRepository)
        {
            _mapper = mapper;
            _bankRepository = bankRepository;
        }

        public async Task<Response<BankModel>> CreateBank(CreateBank model)
        {
            var response = Response<BankModel>.Failed(string.Empty);
            try
            {
                var bankCodeExist = _bankRepository.BankExists(model.BankCode);
                var bankNameExist = _bankRepository.BankExists(model.BankName);
                if (bankCodeExist || bankNameExist)
                {
                    response = Response<BankModel>.Failed("Bank already exists");
                }
                else
                {
                    var mappedBank = _mapper.Map<Bank>(model);
                    mappedBank.DateCreated = DateTime.Now;
                    mappedBank.IsActive = true;

                    await _bankRepository.InsertAsync(mappedBank);

                    response = Response<BankModel>.Success(new BankModel
                    {
                        BankCode = model.BankCode,
                        BankName = model.BankName,
                        ImageUrl = model.ImageUrl,
                        // Add other properties if needed
                    });
                }

            }
            catch(Exception ex)
            {
                response = Response<BankModel>.Failed(ex.Message);
            }
            return await Task.FromResult(response);
        }
        public async Task<Response<BankModel>> GetBankById(int id)
        {
            var response = Response<BankModel>.Failed(string.Empty);
            try
            {
                var entity = _bankRepository.FindBankId(id);
                var mappedBank = _mapper.Map<BankModel>(entity);
                response = Response<BankModel>.Success(mappedBank);
            }catch(Exception ex)
            {
                response = Response<BankModel>.Failed(ex.Message);
            }
            return await Task.FromResult(response);
        }

        public async Task<Response<CountModel<BankItem>>> GetCount(int page, int pagesize, BankFilter filter = null)
        {
            ProcessFilter(filter);
            int totalCount;
            var orderBy = OrderExpression.Deserilizer("{}");
            var entities = _bankRepository.GetBankPaged(page, pagesize, out totalCount, filter, orderBy);

            var response = Response<CountModel<BankItem>>.Success(new CountModel<BankItem>()
            {
                Total = totalCount,
                Items = ProcessQuery(entities)
            });
            return await Task.FromResult(response);
        }
        public async Task<Response<IEnumerable<BankModel>>> BankList()
        {
            var response = Response<IEnumerable<BankModel>>.Failed(string.Empty);
            try
            {
                var entities = _bankRepository.GetBankList();
                var mappedBanks = _mapper.Map<IEnumerable<BankModel>>(entities);
                response = Response<IEnumerable<BankModel>>.Success(mappedBanks);
            }catch(Exception ex)
            {
                response = Response<IEnumerable<BankModel>>.Failed(ex.Message);
            }
            return await Task.FromResult(response);
        }

        public async Task<IEnumerable<BankItem>> Query(int page, int pagesize, BankFilter filter)
        {
            var orderBy = OrderExpression.Deserilizer("{}");
            ProcessFilter(filter);
            var entities = _bankRepository.GetBankPaged(page, pagesize, filter, orderBy);
            var response = ProcessQuery(entities);
            return await Task.FromResult(response);
        }
        private void ProcessFilter(BankFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.BankName))
            {
                var bankItem = _bankRepository.FindByBankCode(filter.BankName);
                if (bankItem != null)
                {
                    filter.BankName = bankItem.BankName;
                }
            }
            if (!string.IsNullOrEmpty(filter.BankCode))
            {
                var bankItem = _bankRepository.FindByBankCode(filter.BankCode);
                if (bankItem != null)
                {
                    filter.BankCode = bankItem.BankCode;
                }
            }
        }
        private IEnumerable<BankItem> ProcessQuery(IEnumerable<Bank> entities)
        {
            return entities.ToList().Select(c =>
            {
                var item = _mapper.Map<Bank, BankItem>(c);
                return item;
            });
        }
    }
}
