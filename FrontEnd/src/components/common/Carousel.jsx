import { useEffect, useState } from "react";
import { FaChevronLeft, FaChevronRight } from "react-icons/fa";

export default function Carousel({
  children,
  autoSlide = false,
  autoSlideInterval = 3000,
}) {
  const slides = children || [];
  const slideCount = slides.length;
  const [current, setCurrent] = useState(0);

  const prev = () =>
    setCurrent((current) => (current === 0 ? slideCount - 1 : current - 1));

  const next = () =>
    setCurrent((current) => (current === slideCount - 1 ? 0 : current + 1));

  useEffect(() => {
    if (!autoSlide || slideCount <= 1) return;
    const slideInterval = setInterval(next, autoSlideInterval);
    return () => clearInterval(slideInterval);
  }, [current, autoSlide, autoSlideInterval, slideCount]);

  if (slideCount === 0) return null;

  return ( 
    <div className="overflow-hidden relative w-full aspect-4/3 rounded-xl shadow-lg bg-gray-900">
      <div
        className="flex transition-transform ease-out duration-500 h-full"
        style={{
          width: `${slideCount * 100}%`,
          transform: `translateX(-${(current / slideCount) * 100}%)`
        }}
      >
        {slides.map((slide, index) => (
          <div
            key={index}
            className="h-full flex shrink-0 items-center justify-center bg-gray-800"
            style={{ width: `${100 / slideCount}%` }}
          > 
            {slide}
          </div>
        ))}
      </div>

      {slideCount > 1 && (
        <div className="absolute inset-0 flex items-center justify-between p-2 z-10">
          <button
            onClick={prev}
            className="p-2 rounded-full shadow-md bg-white/80 text-gray-800 hover:bg-white transition-colors"
          >
            <FaChevronLeft size={24} />
          </button>
          <button
            onClick={next}
            className="p-2 rounded-full shadow-md bg-white/80 text-gray-800 hover:bg-white transition-colors"
          >
            <FaChevronRight size={24} />
          </button>
        </div>
      )}

      {slideCount > 1 && (
        <div className="absolute bottom-3 right-0 left-0 z-10">
          <div className="flex items-center justify-center gap-1.5">
            {slides.map((_, index) => (
              <div
                key={index}
                className={`transition-all w-2 h-2 rounded-full bg-white shadow
                    ${current === index ? "p-1.5" : "bg-opacity-50"}
                  `}
              />
            ))}
          </div>
        </div>
      )}
    </div>
  );
}