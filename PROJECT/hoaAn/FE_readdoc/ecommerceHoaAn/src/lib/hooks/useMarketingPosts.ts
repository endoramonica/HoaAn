/**
 * Hook for managing marketing posts
 */

import { useState, useEffect, useCallback } from 'react';
import {
    getMarketingPosts,
    getPublishedPosts,
    getFeaturedPosts,
    getPostsByProduct,
    getPostsByPlatform,
    searchPosts,
} from '@/lib/services/marketingPostService';
import type { MarketingPost, GetMarketingPostsQuery } from '@/lib/api/types';

interface UseMarketingPostsOptions {
    query?: GetMarketingPostsQuery;
    autoLoad?: boolean;
}

export function useMarketingPosts(options: UseMarketingPostsOptions = {}) {
    const { query = {}, autoLoad = true } = options;

    const [posts, setPosts] = useState<MarketingPost[]>([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [pageNumber, setPageNumber] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [totalItems, setTotalItems] = useState(0);

    // Load posts
    useEffect(() => {
        if (autoLoad) {
            loadPosts();
        }
    }, [pageNumber, query, autoLoad]);

    const loadPosts = async () => {
        setIsLoading(true);
        setError(null);

        try {
            const result = await getMarketingPosts({
                ...query,
                pageNumber,
            });

            setPosts(result.items);
            setTotalPages(result.totalPages);
            setTotalItems(result.totalItems);
        } catch (err: any) {
            setError(err.message || 'Failed to load posts');
            console.error('Error loading posts:', err);
        } finally {
            setIsLoading(false);
        }
    };

    // Load published posts
    const loadPublished = useCallback(async (pageSize = 20) => {
        setIsLoading(true);
        setError(null);

        try {
            const data = await getPublishedPosts(pageSize);
            setPosts(data);
        } catch (err: any) {
            setError(err.message || 'Failed to load published posts');
            console.error('Error loading published posts:', err);
        } finally {
            setIsLoading(false);
        }
    }, []);

    // Load featured posts
    const loadFeatured = useCallback(async (pageSize = 10) => {
        setIsLoading(true);
        setError(null);

        try {
            const data = await getFeaturedPosts(pageSize);
            setPosts(data);
        } catch (err: any) {
            setError(err.message || 'Failed to load featured posts');
            console.error('Error loading featured posts:', err);
        } finally {
            setIsLoading(false);
        }
    }, []);

    // Load posts by product
    const loadByProduct = useCallback(async (productId: string, pageSize = 10) => {
        setIsLoading(true);
        setError(null);

        try {
            const data = await getPostsByProduct(productId, pageSize);
            setPosts(data);
        } catch (err: any) {
            setError(err.message || 'Failed to load posts');
            console.error('Error loading posts by product:', err);
        } finally {
            setIsLoading(false);
        }
    }, []);

    // Load posts by platform
    const loadByPlatform = useCallback(async (platform: string, pageSize = 20) => {
        setIsLoading(true);
        setError(null);

        try {
            const data = await getPostsByPlatform(platform, pageSize);
            setPosts(data);
        } catch (err: any) {
            setError(err.message || 'Failed to load posts');
            console.error('Error loading posts by platform:', err);
        } finally {
            setIsLoading(false);
        }
    }, []);

    // Search posts
    const search = useCallback(async (searchTerm: string, pageSize = 20) => {
        setIsLoading(true);
        setError(null);

        try {
            const data = await searchPosts(searchTerm, pageSize);
            setPosts(data);
        } catch (err: any) {
            setError(err.message || 'Failed to search posts');
            console.error('Error searching posts:', err);
        } finally {
            setIsLoading(false);
        }
    }, []);

    return {
        posts,
        isLoading,
        error,
        pageNumber,
        totalPages,
        totalItems,
        setPageNumber,
        loadPosts,
        loadPublished,
        loadFeatured,
        loadByProduct,
        loadByPlatform,
        search,
    };
}
