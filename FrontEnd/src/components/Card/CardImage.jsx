const CardImage = ({
  src,
  alt = "Card image",
  className = "",
}) => {
  return (
    <div
      className={`w-24 h-24 overflow-hidden rounded-full ${className}`}
    >
      {src ? (
        <img
          src={src}
          alt={alt}
          className="w-full h-full object-cover"
        />
      ) : (
        <div
          className="
            flex h-full w-full items-center justify-center
            bg-linear-to-r from-gray-200 to-gray-400
          "
        >
          <span className="text-sm text-gray-400">
            No image available
          </span>
        </div>
      )}
    </div>
  );
};

export default CardImage;