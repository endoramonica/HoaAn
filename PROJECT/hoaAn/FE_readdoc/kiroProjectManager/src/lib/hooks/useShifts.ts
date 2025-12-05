import { useGetApiShifts, usePostApiShiftsOpen, usePostApiShiftsCloseId, usePutApiShiftsId, useGetApiShiftsCurrentUserId } from '../../../api/generated-orval/shifts/shifts';
import type { GetApiShiftsParams, HRMOpenShiftRequest, HRMCloseShiftRequest, HRMUpdateShiftRequest } from '../../../api/generated-orval/schemas';

export const useShifts = (params?: GetApiShiftsParams) => {
    return useGetApiShifts(params);
};

export const useCurrentShift = (userId: string) => {
    return useGetApiShiftsCurrentUserId(userId);
};

export const useOpenShift = () => {
    return usePostApiShiftsOpen();
};

export const useCloseShift = () => {
    return usePostApiShiftsCloseId();
};

export const useUpdateShift = () => {
    return usePutApiShiftsId();
};
