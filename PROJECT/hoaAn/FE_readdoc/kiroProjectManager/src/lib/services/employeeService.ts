/**
 * Employee Service
 * Service layer for employee management operations
 */

// Mock employee service - replace with real API when available
export const employeeService = {
    async getEmployees(params?: {
        PageNumber?: number;
        PageSize?: number;
        SearchTerm?: string;
    }) {
        // Mock data
        return {
            data: [],
            totalCount: 0,
            pageNumber: params?.PageNumber || 1,
            pageSize: params?.PageSize || 10,
        };
    },

    async getEmployeeById(id: string) {
        return {
            id,
            fullName: '',
            email: '',
            role: 'STAFF',
            isActive: true,
        };
    },

    async createEmployee(data: any) {
        return {
            id: Math.random().toString(36).substr(2, 9),
            ...data,
            createdAt: new Date().toISOString(),
        };
    },

    async updateEmployee(id: string, data: any) {
        return {
            id,
            ...data,
            updatedAt: new Date().toISOString(),
        };
    },

    async deleteEmployee(_id: string) {
        return { success: true };
    },
};
