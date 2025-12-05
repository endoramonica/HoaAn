import { PostCard } from './PostCard';

export const PostList = ({ posts }: { posts: any[] }) => {
  return (
    <div className="space-y-4">
      {posts.map((post, index) => (
        <PostCard 
          key={post.id || post.postId || index} 
          post={post} 
          isNewest={index === 0}
        />
      ))}
    </div>
  );
};
