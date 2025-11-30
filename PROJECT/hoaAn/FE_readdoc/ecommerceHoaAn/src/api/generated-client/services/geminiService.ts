/**
 * Gemini Service - AI Post Generation
 */

import { GoogleGenerativeAI, SchemaType } from "@google/generative-ai";
import type { PostResponseDto } from "../../../../Api/generated-orval/schemas";

const apiKey = import.meta.env.VITE_GEMINI_API_KEY || "";
const ai = new GoogleGenerativeAI(apiKey);

/**
 * Generate AI-powered post suggestions using Gemini
 * @param topic - Topic to generate posts about
 * @param count - Number of posts to generate (default: 3)
 * @returns Array of PostResponseDto
 */
export const generateAiPosts = async (
  topic: string = "spirituality and mindfulness",
  count: number = 3
): Promise<PostResponseDto[]> => {
  if (!apiKey) {
    console.warn("❌ VITE_GEMINI_API_KEY not found, returning mock data.");
    return getMockPosts(count);
  }

  try {
    const model = ai.getGenerativeModel({
      model: "gemini-2.0-flash-exp",
    });

    const prompt = `Generate ${count} realistic Vietnamese social media posts about "${topic}" for a spiritual community platform.

Requirements:
- Write in natural Vietnamese language
- Include mix of short (50-100 words) and medium (100-200 words) posts
- Topics: meditation, mindfulness, spiritual growth, life wisdom, personal development
- Tone: warm, inspiring, authentic
- Use emojis naturally but not excessively
- Make content relatable and engaging

Return ONLY valid JSON array, no markdown formatting.`;

    const response = await model.generateContent({
      contents: [
        {
          role: "user",
          parts: [{ text: prompt }],
        },
      ],
      generationConfig: {
        responseMimeType: "application/json",
        responseSchema: {
          type: SchemaType.ARRAY,
          items: {
            type: SchemaType.OBJECT,
            properties: {
              customerName: { type: SchemaType.STRING },
              content: { type: SchemaType.STRING },
              likesCount: { type: SchemaType.INTEGER },
              commentsCount: { type: SchemaType.INTEGER },
            },
            required: ["customerName", "content"],
          },
        },
      },
    });

    const text = response.response?.text();
    if (!text) {
      throw new Error("Empty response from Gemini");
    }

    const rawData = JSON.parse(text);

    // Transform to PostResponseDto format
    return rawData.map((post: any, index: number): PostResponseDto => {
      const postId = `ai-${Date.now()}-${index}`;
      const customerId = `customer-${Math.random().toString(36).substr(2, 9)}`;
      
      return {
        postId,
        customerId,
        customerName: post.customerName || "Thành viên cộng đồng",
        customerAvatar: `https://api.dicebear.com/7.x/avataaars/svg?seed=${post.customerName || postId}`,
        content: post.content,
        photoUrl: Math.random() > 0.5 
          ? `https://picsum.photos/seed/${postId}/800/600` 
          : null,
        thumbnailUrl: null,
        postedOn: new Date().toISOString(),
        createdAt: new Date().toISOString(),
        updatedAt: null,
        likesCount: post.likesCount || Math.floor(Math.random() * 50),
        commentsCount: post.commentsCount || Math.floor(Math.random() * 20),
        bookmarksCount: Math.floor(Math.random() * 15),
        isLikedByCurrentUser: false,
        isBookmarkedByCurrentUser: false,
        isOwnedByCurrentUser: false,
      };
    });

  } catch (error) {
    console.error("❌ Gemini generation failed:", error);
    console.error("Error details:", error instanceof Error ? error.message : error);
    return getMockPosts(count);
  }
};

/**
 * Fallback mock posts when API fails
 */
function getMockPosts(count: number = 3): PostResponseDto[] {
  const mockContents = [
    "Hôm nay tôi đã có 30 phút thiền định buổi sáng. Cảm giác tâm trí thanh thản và sẵn sàng đón nhận một ngày mới thật tuyệt vời! 🧘‍♀️✨",
    "Chia sẻ một câu nói tôi rất thích: 'Hạnh phúc không phải là điểm đến, mà là cách ta đi.' Mỗi bước chân đều là một phần của hành trình tâm linh. 🌸",
    "Vừa hoàn thành khóa học về mindfulness. Học được rằng việc sống trọn vẹn trong hiện tại giúp ta giảm lo âu và tăng sự biết ơn trong cuộc sống. Ai đã thử chưa? 💚",
    "Buổi sáng đi bộ trong công viên, quan sát thiên nhiên thức giấc. Những khoảnh khắc nhỏ bé này là món quà quý giá của cuộc sống. 🌅🍃",
    "Đọc xong cuốn 'Sức mạnh của hiện tại'. Một quyển sách thay đổi góc nhìn về thời gian và ý thức. Rất recommend cho mọi người! 📚✨",
  ];

  return Array.from({ length: count }, (_, i) => {
    const postId = `mock-${Date.now()}-${i}`;
    const names = ["Minh Anh", "Tuấn Kiệt", "Thùy Linh", "Hoàng Long", "Ngọc Mai"];
    const customerName = names[i % names.length];

    return {
      postId,
      customerId: `customer-${i}`,
      customerName,
      customerAvatar: `https://api.dicebear.com/7.x/avataaars/svg?seed=${customerName}`,
      content: mockContents[i % mockContents.length],
      photoUrl: i % 2 === 0 ? `https://picsum.photos/seed/${postId}/800/600` : null,
      thumbnailUrl: null,
      postedOn: new Date(Date.now() - i * 3600000).toISOString(),
      createdAt: new Date(Date.now() - i * 3600000).toISOString(),
      updatedAt: null,
      likesCount: Math.floor(Math.random() * 100),
      commentsCount: Math.floor(Math.random() * 30),
      bookmarksCount: Math.floor(Math.random() * 20),
      isLikedByCurrentUser: false,
      isBookmarkedByCurrentUser: false,
      isOwnedByCurrentUser: false,
    };
  });
}

/**
 * Helper: Generate post suggestions based on user's writing
 * @param userText - Text user is currently writing
 * @returns Single AI-suggested continuation
 */
export const generatePostSuggestion = async (
  userText: string
): Promise<string> => {
  if (!apiKey || !userText.trim()) {
    return "";
  }

  try {
    const model = ai.getGenerativeModel({
      model: "gemini-2.0-flash-exp",
    });

    const prompt = `User is writing: "${userText}"

Continue this post naturally in Vietnamese for a spiritual community. Keep it:
- Authentic and warm
- 1-2 sentences only
- Natural continuation
- No quotation marks

Return ONLY the continuation text.`;

    const response = await model.generateContent(prompt);
    return response.response?.text()?.trim() || "";

  } catch (error) {
    console.error("Suggestion generation failed:", error);
    return "";
  }
};