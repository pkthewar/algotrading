import { useEffect, useState } from 'react'
import { api } from '../services/api'
import type { PortfolioSnapshot } from '../types/api'

export default function Portfolio() {
  const [portfolio, setPortfolio] = useState<PortfolioSnapshot>({
    cash: 0,
    positions: {},
  })

  useEffect(() => {
    api.get<PortfolioSnapshot>('/trade/getPortfolio').then((res) => {
      setPortfolio(res.data)
    })
  }, [])

  return (
    <div>
      <h2>Portfolio</h2>
      <p>Cash: Rs. {portfolio.cash.toFixed(2)}</p>

      <ul>
        {Object.values(portfolio.positions).map((position) => (
          <li key={position.symbol}>
            {position.symbol} - {position.quantity} @ {position.avgPrice}
          </li>
        ))}
      </ul>
    </div>
  )
}
