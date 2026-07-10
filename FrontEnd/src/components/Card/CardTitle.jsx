const CardTitle = ({
    children,
    className = "",
}) => {
    return (
        <h3
            className={`
        mb-2 whitespace-pre-line text-2xl font-bold
        ${className}
      `}
        >
            {children}
        </h3>
    );
};

export default CardTitle;