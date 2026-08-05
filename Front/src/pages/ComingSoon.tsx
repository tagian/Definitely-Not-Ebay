export default function ComingSoon({ message }: { message?: string }) {
  return (
    <div className="flex flex-col items-center justify-center py-20 text-center">
      <div className="text-4xl mb-4">🚧</div>
      <h2 className="text-2xl font-bold mb-2">
        {message || "Coming Soon"}
      </h2>
      <p className="text-gray-600">
        We’re working hard to bring this feature online. Please check back later!
      </p>
    </div>
  );
}
