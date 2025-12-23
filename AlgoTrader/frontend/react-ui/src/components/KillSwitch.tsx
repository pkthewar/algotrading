import { api } from '../services/api'

export default function KillSwitch() {
    const trigger = async () => {
        await api.post(`/system/kill`)

        alert('Kill Switch Activated')
    }

    return (
        <div>
            <h2>Emergency</h2>
            <button style = {{color: 'red'}} onClick={trigger}>KILL SWITCH</button>
        </div>
    )
}