import { PostCard } from './PostCard';

export const PostList = ({ posts }: { posts: any[] }) => {
  return (
    <div className="space-y-4">
      {posts.map((post) => (
        <PostCard key={post.id} post={post} />
      ))}
    </div>
  );
};
