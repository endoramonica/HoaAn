/**
 * Marketing Service
 * LocalStorage-based service for marketing posts with enhanced features
 */

export interface MarketingPost {
    id: string;
    title: string;
    content: string;
    shortDescription?: string;
    image?: string; // base64 or URL
    images?: string[]; // Multiple images

    // Product relation
    productId?: string;
    productName?: string;

    // Content metadata
    topic?: string;
    platform?: string;
    tone?: 'professional' | 'casual' | 'enthusiastic' | 'friendly';
    hashtags?: string[];

    // SEO
    metaTitle?: string;
    metaDescription?: string;
    metaKeywords?: string[];

    // Social media variants
    socialPosts?: {
        facebook?: string;
        instagram?: string;
        twitter?: string;
        linkedin?: string;
    };

    // Publishing
    status: 'draft' | 'published' | 'scheduled';
    scheduledDate?: string;
    publishedDate?: string;

    // Analytics
    views?: number;
    clicks?: number;
    shares?: number;

    // Timestamps
    createdAt: string;
    updatedAt: string;
    createdBy?: string;
}

const STORAGE_KEY = 'marketing_posts';

export const marketingService = {
    /**
     * Get all posts with optional filters
     */
    async getPosts(filters?: {
        status?: 'draft' | 'published' | 'scheduled';
        productId?: string;
        platform?: string;
        searchTerm?: string;
    }): Promise<MarketingPost[]> {
        const data = localStorage.getItem(STORAGE_KEY);
        let posts: MarketingPost[] = data ? JSON.parse(data) : [];

        // Apply filters
        if (filters) {
            if (filters.status) {
                posts = posts.filter(p => p.status === filters.status);
            }
            if (filters.productId) {
                posts = posts.filter(p => p.productId === filters.productId);
            }
            if (filters.platform) {
                posts = posts.filter(p => p.platform === filters.platform);
            }
            if (filters.searchTerm) {
                const term = filters.searchTerm.toLowerCase();
                posts = posts.filter(p =>
                    p.title.toLowerCase().includes(term) ||
                    p.content.toLowerCase().includes(term) ||
                    p.productName?.toLowerCase().includes(term)
                );
            }
        }

        // Sort by updatedAt desc
        return posts.sort((a, b) =>
            new Date(b.updatedAt).getTime() - new Date(a.updatedAt).getTime()
        );
    },

    /**
     * Get single post by ID
     */
    async getPost(id: string): Promise<MarketingPost | null> {
        const posts = await this.getPosts();
        return posts.find(p => p.id === id) || null;
    },

    /**
     * Create new post
     */
    async createPost(post: Omit<MarketingPost, 'id' | 'createdAt' | 'updatedAt'>): Promise<MarketingPost> {
        const posts = await this.getPosts();
        const newPost: MarketingPost = {
            ...post,
            id: crypto.randomUUID(),
            views: 0,
            clicks: 0,
            shares: 0,
            createdAt: new Date().toISOString(),
            updatedAt: new Date().toISOString(),
        };
        posts.push(newPost);
        localStorage.setItem(STORAGE_KEY, JSON.stringify(posts));
        return newPost;
    },

    /**
     * Update existing post
     */
    async updatePost(id: string, data: Partial<MarketingPost>): Promise<MarketingPost> {
        const posts = await this.getPosts();
        const index = posts.findIndex(p => p.id === id);
        if (index === -1) throw new Error('Post not found');

        posts[index] = {
            ...posts[index],
            ...data,
            updatedAt: new Date().toISOString(),
        };
        localStorage.setItem(STORAGE_KEY, JSON.stringify(posts));
        return posts[index];
    },

    /**
     * Delete post
     */
    async deletePost(id: string): Promise<void> {
        const posts = await this.getPosts();
        const filtered = posts.filter(p => p.id !== id);
        localStorage.setItem(STORAGE_KEY, JSON.stringify(filtered));
    },

    /**
     * Publish post (change status to published)
     */
    async publishPost(id: string): Promise<MarketingPost> {
        return this.updatePost(id, {
            status: 'published',
            publishedDate: new Date().toISOString(),
        });
    },

    /**
     * Schedule post
     */
    async schedulePost(id: string, scheduledDate: string): Promise<MarketingPost> {
        return this.updatePost(id, {
            status: 'scheduled',
            scheduledDate,
        });
    },

    /**
     * Duplicate post
     */
    async duplicatePost(id: string): Promise<MarketingPost> {
        const post = await this.getPost(id);
        if (!post) throw new Error('Post not found');

        const { id: _, createdAt, updatedAt, publishedDate, views, clicks, shares, ...postData } = post;

        return this.createPost({
            ...postData,
            title: `${post.title} (Copy)`,
            status: 'draft',
        });
    },

    /**
     * Get posts by product
     */
    async getPostsByProduct(productId: string): Promise<MarketingPost[]> {
        return this.getPosts({ productId });
    },

    /**
     * Get posts statistics
     */
    async getStatistics(): Promise<{
        total: number;
        draft: number;
        published: number;
        scheduled: number;
        totalViews: number;
        totalClicks: number;
        totalShares: number;
    }> {
        const posts = await this.getPosts();

        return {
            total: posts.length,
            draft: posts.filter(p => p.status === 'draft').length,
            published: posts.filter(p => p.status === 'published').length,
            scheduled: posts.filter(p => p.status === 'scheduled').length,
            totalViews: posts.reduce((sum, p) => sum + (p.views || 0), 0),
            totalClicks: posts.reduce((sum, p) => sum + (p.clicks || 0), 0),
            totalShares: posts.reduce((sum, p) => sum + (p.shares || 0), 0),
        };
    },

    /**
     * Increment view count
     */
    async incrementViews(id: string): Promise<void> {
        const post = await this.getPost(id);
        if (post) {
            await this.updatePost(id, {
                views: (post.views || 0) + 1,
            });
        }
    },

    /**
     * Increment click count
     */
    async incrementClicks(id: string): Promise<void> {
        const post = await this.getPost(id);
        if (post) {
            await this.updatePost(id, {
                clicks: (post.clicks || 0) + 1,
            });
        }
    },

    /**
     * Increment share count
     */
    async incrementShares(id: string): Promise<void> {
        const post = await this.getPost(id);
        if (post) {
            await this.updatePost(id, {
                shares: (post.shares || 0) + 1,
            });
        }
    },

    /**
     * Export posts to JSON
     */
    async exportPosts(): Promise<string> {
        const posts = await this.getPosts();
        return JSON.stringify(posts, null, 2);
    },

    /**
     * Import posts from JSON
     */
    async importPosts(jsonData: string): Promise<number> {
        const importedPosts: MarketingPost[] = JSON.parse(jsonData);
        const existingPosts = await this.getPosts();

        // Merge posts (avoid duplicates by ID)
        const existingIds = new Set(existingPosts.map(p => p.id));
        const newPosts = importedPosts.filter(p => !existingIds.has(p.id));

        const allPosts = [...existingPosts, ...newPosts];
        localStorage.setItem(STORAGE_KEY, JSON.stringify(allPosts));

        return newPosts.length;
    },

    /**
     * Clear all posts (use with caution)
     */
    async clearAllPosts(): Promise<void> {
        localStorage.removeItem(STORAGE_KEY);
    },
};
