class ATCar : Car {
    public new void Accelerator() {
        if (6 == this.Gear) { this.Gear = 0; }
        this.Gear++;
        base.Accelerator();
    }
}
