namespace Network
{
    public enum AuthOpcode
    {
        NoOp=0,
        AuthIdent=1,
        AuthChallenge=2,
        AuthProof=3,
        AuthProofAck=4,
        AuthProofAckAck=5,
        AuthComplete=6,
    }
}