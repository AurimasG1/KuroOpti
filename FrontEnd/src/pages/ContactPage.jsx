import React from "react";
import Card from "../components/Card/Card";
import CardImage from "../components/Card/CardImage";
import { carsdData } from "../data/cardsData";
import CardHeader from "../components/Card/CardHeader";
import { span } from "framer-motion/client";
import CardTitle from "../components/Card/CardTitle";
import CardBody from "../components/Card/CardBody";
import CardFooter from "../components/Card/CardFooter";

const ContactPage = () => {
  return (
    <div className="min-h-screen p-8">
      <div className="max-w-7xl mx-auto">
        <div className="text-center mb-12">
          <h1 className="text-5xl font-bold text-gray-800 mb-4 text-nowrap">
            CTRL ALT DELETE
          </h1>
          <p className="text-xl text-gray-600">Susipažink su mūsų komanda:</p>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8 ">
          {carsdData.map((card) => (
            <Card
              key={card.id}
              // variant={card.id % 2 === 0 ? "primary" : "default"}
            variant="glass"
              padding="none"
            >
              <div className="p-6">
                <div className="flex flex-row gap-4">
                  {card.image && (
                    <CardImage src={card.image} alt={card.title} />
                  )}
                  <CardHeader>
                    {card.category && (
                      <span className="inline-block px-3 py-1 bg-blue-100 text-blue-700 text-sm font-semibold rounded-full mb-2">
                        {card.category}
                      </span>
                    )}
                    <CardTitle
                      className={card.id % 2 === 0 ? "text-blue-800" : ""}
                    >
                      {card.title}
                    </CardTitle>
                   
                  </CardHeader>
                </div>

                {card.description ? (
                  <CardBody
                    className={card.id % 2 === 0 ? "text-blue-700" : ""}
                  >
                    {card.description}
                  </CardBody>
                ) : (
                  <CardBody className="text-gray-800 italic">
                    No description for this destination
                  </CardBody>
                )}
                <CardFooter
                  className={card.id % 2 === 0 ? "border-blue-200" : ""}
                >
                  
                    <button className="w-full px-5 py-2 bg-blue-500 text-white rounded-lg hover:bg-blue-700 transition-colors font-medium">
                        <a href={card.contact} target="_blank">Contact Now</a>
                      
                    </button>
                  
                </CardFooter>
              </div>
            </Card>
          ))}
        </div>
      </div>
    </div>
  );
};

export default ContactPage;
