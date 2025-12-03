using AutoMapper;
using Microsoft.Extensions.Logging;
using WebStore.DAL.EF;
namespace WebStore.Services.ConcreteServices {
public abstract class BaseService {
protected readonly WebStoreDbContext DbContext;
protected readonly ILogger Logger;
protected readonly IMapper Mapper;
public BaseService (WebStoreDbContext dbContext,
IMapper mapper, ILogger logger) {
DbContext = dbContext;
Logger = logger;
Mapper = mapper;
}
}
}
