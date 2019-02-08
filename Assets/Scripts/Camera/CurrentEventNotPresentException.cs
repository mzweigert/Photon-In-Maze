
using System;

public class CurrentEventNotPresentException : Exception {
   
    public CurrentEventNotPresentException() : base("Current event is not present") {
        
    }
}
