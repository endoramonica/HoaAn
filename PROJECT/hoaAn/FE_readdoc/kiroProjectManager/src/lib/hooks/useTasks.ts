import { useGetApiTask, useGetApiTaskId, usePostApiTask, usePutApiTaskId, useDeleteApiTaskId, usePatchApiTaskIdStatus } from '../../../api/generated-orval/task/task';
import type { GetApiTaskParams, TasksCreateTaskRequest, TasksUpdateTaskRequest, EntitiesTasksTaskStatus } from '../../../api/generated-orval/schemas';

export const useTasks = (params?: GetApiTaskParams) => {
    return useGetApiTask(params);
};

export const useTask = (id: string) => {
    return useGetApiTaskId(id);
};

export const useCreateTask = () => {
    return usePostApiTask();
};

export const useUpdateTask = () => {
    return usePutApiTaskId();
};

export const useDeleteTask = () => {
    return useDeleteApiTaskId();
};

export const useUpdateTaskStatus = () => {
    return usePatchApiTaskIdStatus();
};
