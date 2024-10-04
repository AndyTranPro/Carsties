'use client'

import React, { useState } from 'react'
import Image from 'next/image'

type Props = {
    imageUrl: string;
}

export default function CarImage({imageUrl}: Props) {
    const [isLoading, setIsLoading] = useState(true);
    return (
        <Image 
            src={imageUrl}
            alt='image of car'
            fill
            priority
            className={`
                    object-cover 
                    group-hover:opacity-75 
                    duration-700 
                    ease-in-out
                    ${isLoading 
                        ? 'grayscale blur-2xl scale-100' 
                        : 'grayscale-0 blur-0 scale-100'}
                `}
            sizes='(max-width: 768px) 100vw, (max-width: 1200px) 50vw, 25vw'
            onLoad={() => setIsLoading(false)}
        />
    )
}
