export interface ProductFilterDto {
    pageNumber?: number;
    pageSize?: number;
    searchTerm?: string;
    categoryId?: string;
    storeId?: string;
    isActive?: boolean;
    minPrice?: number;
    maxPrice?: number;
    sortBy?: string;
    isDescending?: boolean;
  }