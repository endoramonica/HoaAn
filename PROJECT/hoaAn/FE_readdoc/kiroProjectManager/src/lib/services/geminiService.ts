/**
 * Gemini AI Service
 * Service để tương tác với Google Gemini API
 */

const GEMINI_API_KEY = import.meta.env.VITE_GEMINI_API_KEY;
const GEMINI_API_URL = 'https://generativelanguage.googleapis.com/v1beta';

interface GenerateTextOptions {
    prompt: string;
    model?: string;
    temperature?: number;
    maxTokens?: number;
}

interface GenerateImageOptions {
    prompt: string;
    numberOfImages?: number;
}

/**
 * Generate text content using Gemini
 */
export async function generateText(options: GenerateTextOptions): Promise<string> {
    const {
        prompt,
        model = 'gemini-2.5-flash',
        temperature = 0.7,
        maxTokens = 8192,
    } = options;

    if (!GEMINI_API_KEY) {
        throw new Error('VITE_GEMINI_API_KEY không được cấu hình trong .env');
    }

    try {
        const response = await fetch(
            `${GEMINI_API_URL}/models/${model}:generateContent?key=${GEMINI_API_KEY}`,
            {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    contents: [
                        {
                            parts: [
                                {
                                    text: prompt,
                                },
                            ],
                        },
                    ],
                    generationConfig: {
                        temperature,
                        maxOutputTokens: maxTokens,
                        topK: 40,
                        topP: 0.95,
                    },
                    safetySettings: [
                        {
                            category: 'HARM_CATEGORY_HARASSMENT',
                            threshold: 'BLOCK_MEDIUM_AND_ABOVE',
                        },
                        {
                            category: 'HARM_CATEGORY_HATE_SPEECH',
                            threshold: 'BLOCK_MEDIUM_AND_ABOVE',
                        },
                        {
                            category: 'HARM_CATEGORY_SEXUALLY_EXPLICIT',
                            threshold: 'BLOCK_MEDIUM_AND_ABOVE',
                        },
                        {
                            category: 'HARM_CATEGORY_DANGEROUS_CONTENT',
                            threshold: 'BLOCK_MEDIUM_AND_ABOVE',
                        },
                    ],
                }),
            }
        );

        if (!response.ok) {
            const errorText = await response.text();
            let errorMessage = 'Lỗi khi gọi Gemini API';

            try {
                const error = JSON.parse(errorText);
                errorMessage = error.error?.message || errorMessage;
            } catch {
                errorMessage = `${errorMessage} (${response.status})`;
            }

            throw new Error(errorMessage);
        }

        const data = await response.json();
        const text = data.candidates?.[0]?.content?.parts?.[0]?.text;

        if (!text) {
            throw new Error('Không nhận được nội dung từ Gemini');
        }

        return text;
    } catch (error: any) {
        console.error('Gemini API Error:', error);
        throw new Error(error.message || 'Lỗi khi gọi Gemini API');
    }
}

/**
 * Generate marketing content for a product
 */
export async function generateMarketingContent(productInfo: {
    name: string;
    description?: string;
    price?: number;
    category?: string;
    targetAudience?: string;
    tone?: 'professional' | 'casual' | 'enthusiastic' | 'friendly';
}): Promise<{
    title: string;
    content: string;
    hashtags: string[];
}> {
    const { name, description, price, category, targetAudience, tone = 'friendly' } = productInfo;

    const toneDescriptions = {
        professional: 'chuyên nghiệp, trang trọng',
        casual: 'thân thiện, gần gũi',
        enthusiastic: 'nhiệt tình, hào hứng',
        friendly: 'thân thiện, ấm áp',
    };

    const prompt = `
Bạn là một chuyên gia marketing. Hãy tạo nội dung quảng cáo cho sản phẩm sau:

Tên sản phẩm: ${name}
${description ? `Mô tả: ${description}` : ''}
${price ? `Giá: ${price.toLocaleString('vi-VN')} VND` : ''}
${category ? `Danh mục: ${category}` : ''}
${targetAudience ? `Đối tượng khách hàng: ${targetAudience}` : ''}

Yêu cầu:
- Tạo 1 tiêu đề hấp dẫn (tối đa 100 ký tự)
- Tạo nội dung bài viết marketing (200-300 từ)
- Giọng điệu: ${toneDescriptions[tone]}
- Tạo 5-7 hashtags phù hợp
- Viết bằng tiếng Việt
- Tập trung vào lợi ích của khách hàng
- Kêu gọi hành động rõ ràng

Định dạng trả về:
TITLE: [tiêu đề]
CONTENT: [nội dung]
HASHTAGS: [hashtag1, hashtag2, hashtag3, ...]
`;

    const response = await generateText({ prompt, temperature: 0.8 });

    // Parse response
    const titleMatch = response.match(/TITLE:\s*(.+?)(?=\n|CONTENT:)/s);
    const contentMatch = response.match(/CONTENT:\s*(.+?)(?=\n|HASHTAGS:)/s);
    const hashtagsMatch = response.match(/HASHTAGS:\s*(.+?)$/s);

    const title = titleMatch?.[1]?.trim() || name;
    const content = contentMatch?.[1]?.trim() || response;
    const hashtagsText = hashtagsMatch?.[1]?.trim() || '';
    const hashtags = hashtagsText
        .split(',')
        .map((tag) => tag.trim().replace(/^#/, ''))
        .filter(Boolean);

    return {
        title,
        content,
        hashtags: hashtags.length > 0 ? hashtags : ['sale', 'shopping', 'deal'],
    };
}

/**
 * Generate social media post variations
 */
export async function generateSocialMediaPosts(
    baseContent: string,
    platforms: ('facebook' | 'instagram' | 'twitter' | 'linkedin')[]
): Promise<Record<string, string>> {
    const platformDescriptions = {
        facebook: 'Facebook (dài, chi tiết, có emoji)',
        instagram: 'Instagram (ngắn gọn, nhiều emoji, hashtags)',
        twitter: 'Twitter/X (ngắn gọn, tối đa 280 ký tự)',
        linkedin: 'LinkedIn (chuyên nghiệp, trang trọng)',
    };

    const prompt = `
Dựa trên nội dung marketing sau:
"${baseContent}"

Hãy tạo các phiên bản phù hợp cho từng nền tảng mạng xã hội:
${platforms.map((p) => `- ${platformDescriptions[p]}`).join('\n')}

Định dạng trả về:
${platforms.map((p) => `${p.toUpperCase()}: [nội dung]`).join('\n')}
`;

    const response = await generateText({ prompt, temperature: 0.7 });

    const result: Record<string, string> = {};
    platforms.forEach((platform) => {
        const regex = new RegExp(`${platform.toUpperCase()}:\\s*(.+?)(?=\\n[A-Z]+:|$)`, 's');
        const match = response.match(regex);
        result[platform] = match?.[1]?.trim() || baseContent;
    });

    return result;
}

/**
 * Improve existing marketing content
 */
export async function improveMarketingContent(
    currentContent: string,
    improvements: string[]
): Promise<string> {
    const prompt = `
Nội dung marketing hiện tại:
"${currentContent}"

Hãy cải thiện nội dung này với các yêu cầu sau:
${improvements.map((imp, i) => `${i + 1}. ${imp}`).join('\n')}

Trả về nội dung đã được cải thiện, giữ nguyên định dạng và độ dài tương tự.
`;

    return generateText({ prompt, temperature: 0.6 });
}

/**
 * Generate product description from basic info
 */
export async function generateProductDescription(productInfo: {
    name: string;
    category?: string;
    features?: string[];
    benefits?: string[];
}): Promise<string> {
    const { name, category, features = [], benefits = [] } = productInfo;

    const prompt = `
Tạo mô tả sản phẩm hấp dẫn cho:

Tên: ${name}
${category ? `Danh mục: ${category}` : ''}
${features.length > 0 ? `Tính năng:\n${features.map((f) => `- ${f}`).join('\n')}` : ''}
${benefits.length > 0 ? `Lợi ích:\n${benefits.map((b) => `- ${b}`).join('\n')}` : ''}

Yêu cầu:
- Mô tả ngắn gọn, súc tích (100-150 từ)
- Tập trung vào lợi ích khách hàng
- Sử dụng ngôn ngữ hấp dẫn, thuyết phục
- Viết bằng tiếng Việt
`;

    return generateText({ prompt, temperature: 0.7 });
}

/**
 * Generate SEO-friendly content
 */
export async function generateSEOContent(productInfo: {
    name: string;
    description: string;
    keywords?: string[];
}): Promise<{
    metaTitle: string;
    metaDescription: string;
    metaKeywords: string[];
}> {
    const { name, description, keywords = [] } = productInfo;

    const prompt = `
Tạo nội dung SEO cho sản phẩm:

Tên: ${name}
Mô tả: ${description}
${keywords.length > 0 ? `Từ khóa gợi ý: ${keywords.join(', ')}` : ''}

Yêu cầu:
- Meta Title: Tối đa 60 ký tự, hấp dẫn, có từ khóa chính
- Meta Description: 150-160 ký tự, mô tả ngắn gọn, kêu gọi hành động
- Meta Keywords: 5-10 từ khóa liên quan

Định dạng:
TITLE: [meta title]
DESCRIPTION: [meta description]
KEYWORDS: [keyword1, keyword2, ...]
`;

    const response = await generateText({ prompt, temperature: 0.6 });

    const titleMatch = response.match(/TITLE:\s*(.+?)(?=\n|DESCRIPTION:)/s);
    const descMatch = response.match(/DESCRIPTION:\s*(.+?)(?=\n|KEYWORDS:)/s);
    const keywordsMatch = response.match(/KEYWORDS:\s*(.+?)$/s);

    return {
        metaTitle: titleMatch?.[1]?.trim() || name,
        metaDescription: descMatch?.[1]?.trim() || description.substring(0, 160),
        metaKeywords: keywordsMatch?.[1]
            ?.split(',')
            .map((k) => k.trim())
            .filter(Boolean) || keywords,
    };
}
