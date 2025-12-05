/**
 * AI Service
 * Integration with Gemini and OpenAI for content generation
 */

interface AIGenerateTextRequest {
    prompt: string;
    model: 'gemini' | 'openai';
}

interface AIGenerateImageRequest {
    prompt: string;
}

export const aiService = {
    async generateText(request: AIGenerateTextRequest): Promise<string> {
        if (request.model === 'gemini') {
            return this.generateTextWithGemini(request.prompt);
        } else {
            return this.generateTextWithOpenAI(request.prompt);
        }
    },

    async generateTextWithGemini(prompt: string): Promise<string> {
        const apiKey = import.meta.env.VITE_GEMINI_API_KEY;

        if (!apiKey) {
            throw new Error('VITE_GEMINI_API_KEY not configured');
        }

        try {
            const response = await fetch(
                `https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash-exp:generateContent?key=${apiKey}`,
                {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({
                        contents: [{
                            parts: [{
                                text: prompt
                            }]
                        }]
                    }),
                }
            );

            if (!response.ok) {
                throw new Error(`Gemini API error: ${response.statusText}`);
            }

            const data = await response.json();
            return data.candidates[0]?.content?.parts[0]?.text || '';
        } catch (error) {
            console.error('Gemini API error:', error);
            throw error;
        }
    },

    async generateTextWithOpenAI(prompt: string): Promise<string> {
        const apiKey = import.meta.env.VITE_OPENAI_API_KEY;

        if (!apiKey) {
            throw new Error('VITE_OPENAI_API_KEY not configured');
        }

        try {
            const response = await fetch('https://api.openai.com/v1/chat/completions', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${apiKey}`,
                },
                body: JSON.stringify({
                    model: 'gpt-4o-mini',
                    messages: [
                        {
                            role: 'user',
                            content: prompt
                        }
                    ],
                }),
            });

            if (!response.ok) {
                throw new Error(`OpenAI API error: ${response.statusText}`);
            }

            const data = await response.json();
            return data.choices[0]?.message?.content || '';
        } catch (error) {
            console.error('OpenAI API error:', error);
            throw error;
        }
    },

    async generateImage(request: AIGenerateImageRequest): Promise<string> {
        const apiKey = import.meta.env.VITE_GEMINI_API_KEY;

        if (!apiKey) {
            throw new Error('VITE_GEMINI_API_KEY not configured');
        }

        try {
            const response = await fetch(
                `https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash-exp:generateContent?key=${apiKey}`,
                {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({
                        contents: [{
                            parts: [{
                                text: `Generate an image: ${request.prompt}`
                            }]
                        }],
                        generationConfig: {
                            responseModalities: ['image']
                        }
                    }),
                }
            );

            if (!response.ok) {
                throw new Error(`Gemini Image API error: ${response.statusText}`);
            }

            const data = await response.json();
            // Return base64 image
            return data.candidates[0]?.content?.parts[0]?.inlineData?.data || '';
        } catch (error) {
            console.error('Gemini Image API error:', error);
            throw error;
        }
    },
};
