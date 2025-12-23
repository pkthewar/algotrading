import { useEffect, useState } from 'react'
import { api } from '../services/api'

type Position = {
    symbol : string
    quantity: number
    avgPrice: number
}

export default function Portfolio() {

    const[cash, setCash] = useState<number>(0)
    const[positions, setPositions] = useState<Record<string, Position>>({})

    useEffect(() => {
        api.get(`/trade/portfolio`).then((res) => {
            setCash(res.data.cash)
            setPositions(res.data.positions)
        })
    }, [])

    return (
        <div>
            <h2>Portfolio</h2>
            <p>Cash: Rs. {cash.toFixed(2)}</p>

            <ul>
                {Object.values(positions).map((p) => (
                    <li key = {p.symbol}>
                        {p.symbol} - {p.quantity} @ {p.avgPrice}
                    </li>
                ))}
            </ul>
        </div>
    )
}