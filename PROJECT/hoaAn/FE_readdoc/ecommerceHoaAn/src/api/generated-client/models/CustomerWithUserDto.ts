/* generated using openapi-typescript-codegen -- do not edit */
/* istanbul ignore file */
/* tslint:disable */
/* eslint-disable */
import type { CustomerDetailDto } from './CustomerDetailDto';
import type { UserDetailDTO } from './UserDetailDTO';
/**
 * DTO for combined Customer + User information
 * Used in GET /api/v1/AdminCustomer/{id} endpoint
 */
export type CustomerWithUserDto = {
    customer?: CustomerDetailDto;
    user?: UserDetailDTO;
};

