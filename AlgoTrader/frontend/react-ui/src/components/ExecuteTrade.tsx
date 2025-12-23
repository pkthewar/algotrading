import { api } from '../services/api'

export const TradeAction = {
    Buy: 0,
    Sell: 1,
    Hold: 2
} as const

export type TradeAction = typeof TradeAction[keyof typeof TradeAction]

type TradeRequest = {
    Symbol: string,
    Quantity: number,
    TradeAction: TradeAction,
    Price: number
}

export default function ExecuteTrade() {
    const exec = async (tradeRequest: TradeRequest) => {
        await api.post<TradeRequest>(`/trade/execute`, tradeRequest)
    }

    return (
        <div>
            <h2>Execute Trade</h2>
        </div>

    )
}