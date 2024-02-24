﻿using GameStore.Domain.Entities;

namespace GameStore.Domain.ISearchCriterias;

public interface IGenresSearchCriteria
{
    public Task<IEnumerable<Genre>> GetByGameKey(string gameKey);

    public Task<IEnumerable<Genre>> GetByParentId(Guid parentId);
}
