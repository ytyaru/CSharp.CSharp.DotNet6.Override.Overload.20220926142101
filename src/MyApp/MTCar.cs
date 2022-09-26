class MTCar : Car {
    public void Accelerator(int gear) {
        if (1 == Math.Abs(this.Gear - gear)) {
            Console.WriteLine("{0}.{1}({2}) {3}", 
                GetType().Name, 
                System.Reflection.MethodBase.GetCurrentMethod().Name, 
                gear, this.Gear);
            this.Gear = gear;
        }
        else {
            Console.WriteLine("{0}.{1}({2}) {3} {4}", 
                GetType().Name, 
                System.Reflection.MethodBase.GetCurrentMethod().Name, 
                gear, this.Gear, "エンスト！");
            this.Gear = 0;
        }
    }
}
