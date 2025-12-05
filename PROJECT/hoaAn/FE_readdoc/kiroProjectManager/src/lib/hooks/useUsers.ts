import { useGetApiUsers } from '../../../api/generated-orval/users/users';

export const useUsers = (params?: { pageNumber?: number; pageSize?: number }) => {
    return useGetApiUsers(params);
};
