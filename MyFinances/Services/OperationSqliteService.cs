using MyFinances.Core.Dtos;
using MyFinances.Core.Response;
using MyFinances.Core;
using MyFinances.Models.Converters;
using MyFinances.Models.Domains;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MyFinances.Models;

namespace MyFinances.Services
{
    internal class OperationSqliteService : IOperationService
    {
        private static UnitOfWork _unitOfWork;

        public static UnitOfWork UnitOfWork
        {
            get
            {
                if (_unitOfWork == null)
                {
                    _unitOfWork = new UnitOfWork(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MyFinancesSQLite.db3"));
                }
                return _unitOfWork;
            }
        }


        public async Task<DataResponse<int>> AddAsync(OperationDto operation)
        {
            var response = new DataResponse<int>();

            try
            {
                response.Data = await UnitOfWork.OperationRepository.AddAsync(operation.ToDao());
            }
            catch (Exception exception)
            {
                response.Errors.Add(new Error(exception.Source, exception.Message));
            }

            return response;
        }

        public async Task<Response> DeleteAsync(int id)
        {
            var response = new Response();

            try
            {
                await UnitOfWork.OperationRepository.DeleteAsync(new Operation { Id = id });
            }
            catch (Exception exception)
            {
                response.Errors.Add(new Error(exception.Source, exception.Message));
            }

            return response;
        }

        public async Task<DataResponse<OperationDto>> GetAsync(int id)
        {
            var response = new DataResponse<OperationDto>();

            try
            {
                response.Data = (await UnitOfWork.OperationRepository.GetAsync(id)).ToDto();
            }
            catch (Exception exception)
            {
                response.Errors.Add(new Error(exception.Source, exception.Message));
            }

            return response;
        }

        public async Task<DataResponse<IEnumerable<OperationDto>>> GetAsync()
        {
            var response = new DataResponse<IEnumerable<OperationDto>>();

            try
            {
                response.Data = (await UnitOfWork.OperationRepository.GetAsync()).ToDtos();
            }
            catch (Exception exception)
            {
                response.Errors.Add(new Error(exception.Source, exception.Message));
            }

            return response;
        }

        public async Task<DataResponse<IEnumerable<OperationDto>>> GetAsync(PaginationFilter paginationFilter)
        {
            var response = new DataResponse<IEnumerable<OperationDto>>();

            try
            {
                response.Data = (await UnitOfWork.OperationRepository.GetAsync(paginationFilter)).ToDtos();
            }
            catch (Exception exception)
            {
                response.Errors.Add(new Error(exception.Source, exception.Message));
            }

            return response;
        }

        public async Task<Response> UpdateAsync(OperationDto operation)
        {
            var response = new Response();

            try
            {
                await UnitOfWork.OperationRepository.UpdateAsync(operation.ToDao());
            }
            catch (Exception exception)
            {
                response.Errors.Add(new Error(exception.Source, exception.Message));
            }

            return response;
        }
    }
}
