 public abstract class GameLife
 {
     public readonly byte MaxLife;
     public byte Life { get; private set; }

     public GameLife(byte maxLife)
     {
         MaxLife = maxLife;
         Reset();
     }

     public void Reset()
     {
         Life = MaxLife;
     }

     public bool AddLifeAndCheckMax(byte amount)
     {
         int newLife = Life + amount;
         if (newLife >= MaxLife)
         {
             Life = MaxLife;
             return true;
         }

         Life = (byte)newLife;
         return false;
     }

     public bool RemoveLifeAndCheckZero(byte amount)
     {
         int newLife = Life - amount;
         if (newLife <= 0)
         {
             Life = 0;
             return true;
         }

         Life = (byte)newLife;
         return false;
     }
 }