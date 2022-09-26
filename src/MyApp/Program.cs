Car car = new Car();
ATCar at = new ATCar();
MTCar mt = new MTCar();

car.Accelerator();
car.Accelerator();

Console.WriteLine("---------- AT ----------");

at.Accelerator();
at.Accelerator();
at.Accelerator();
at.Accelerator();
at.Accelerator();
at.Accelerator();
at.Accelerator();

Console.WriteLine("---------- MT ----------");
mt.Accelerator();
mt.Accelerator();
Console.WriteLine("---------- MT 引数あり ----------");
mt.Accelerator(1);
mt.Accelerator(2);
mt.Accelerator(3);
mt.Accelerator(2);
mt.Accelerator(4);
mt.Accelerator(3);
mt.Accelerator(1);

Console.WriteLine("========== Car ==========");

Car at1 = new ATCar();
Car mt1 = new MTCar();
at1.Accelerator();
at1.Accelerator();
//mt1.Accelerator(1); // error CS1501: 引数 1 を指定するメソッド 'Accelerator' のオーバーロードはありません
//mt1.Accelerator(2); //  error CS1501: 引数 1 を指定するメソッド 'Accelerator' のオーバーロードはありません

Console.WriteLine("========== IAccelerator ==========");

IAccelerator at2 = new ATCar();
IAccelerator mt2 = new MTCar();
at2.Accelerator();
at2.Accelerator();
//mt2.Accelerator(1); // error CS1501: 引数 1 を指定するメソッド 'Accelerator' のオーバーロードはありません
//mt2.Accelerator(2); // error CS1501: 引数 1 を指定するメソッド 'Accelerator' のオーバーロードはありません

