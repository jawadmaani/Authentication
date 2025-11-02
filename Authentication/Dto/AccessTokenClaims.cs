namespace Authentication.Dto
{
    public class AccessTokenClaims
    {
        public string Issuer { get; set; }        
        public string Subject { get; set; }       
        public string Audience { get; set; }      
        public DateTime IssuedAt { get; set; }    
        public DateTime ExpiresAt { get; set; }  
        public string JwtId { get; set; }        
        public string Role { get; set; }          
       
    }
}