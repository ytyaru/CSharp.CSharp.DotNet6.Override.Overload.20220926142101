# dotnet6でオーバーロードする

　クラスメソッドのオーバーライド、オーバーロードをしてみる。

<!-- more -->

# ブツ

* [][]

[]:

# 経緯

　[][]のときJavaScriptでオーバーロードできない問題があった。あのとき考えていた設計をC#で書いてみる。

[]:

　ようするに同じメソッド名でありながら、引数がない場合とある場合の2種類がある。それぞれの実装は別にあり、異なるクラスで書きたい。その抽象化をやってみる。

## 題材

　今回は車を題材にしてみる。AT車とMT車でおなじ動作であるアクセルを踏む`Accelerator()`を実装する。ただしその内容は異なる。AT車はギアを自動できりかえる。MT車はギアを手動できりかえる。このときギアを引数として受けとるようにする。この引数を受けとるかどうかが両者のちがい。

種類|アクセルを踏む
----|--------------
AT|`Accelerator()`
MT|`Accelerator(int gear)`

# プロジェクト作成

```sh
NAME=MyApp
dotnet new console -o $NAME -f net6.0
cd $NAME
```

## MyApp.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>disable</Nullable><!-- enable, disable -->
  </PropertyGroup>

</Project>
```

　`<Nullable>`を`disable`にしておく。デフォルトは`enable`だったが、実行すると以下のような警告が出たので。

```sh
/tmp/work/MyApp/Car.cs(9,60): warning CS8602: null 参照の可能性があるものの逆参照です。 [/tmp/work/MyApp/MyApp.csproj]
```

# ソースコード

## IAccelerator.cs

```csharp
interface IAccelerator {
    void Accelerator();
}
```

　`Accelerator()`メソッドをもたせて共通化するためのインタフェース。

## Car.cs

```csharp
class Car : IAccelerator {
    private int gear;
    protected int Gear {
        get { return this.gear; }
        set { if (0<=value && value<=6) { this.gear = value; } }
    }
    public void Accelerator() {
        Console.WriteLine("{0}.{1}() {2}", GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name, this.Gear);
    }
}
```

　車クラス。`Accelerator()`を実装する。呼び出すとクラス名、メソッド名、現在のギアを表示する。

　ギアは`0`〜`6`の整数をセットできる。

## ATCar.cs

```csharp
class ATCar : Car {
    public new void Accelerator() {
        if (6 == this.Gear) { this.Gear = 0; }
        this.Gear++;
        base.Accelerator();
    }
}
```

　AT車。`Car`を継承する。`Accelerator()`をオーバーライドする。`new`キーワードをつけることで親の同名メソッドでなくこのメソッドを呼び出すようにする。`new`を明示しないとビルド時に以下のような警告がでる。

```sh
/tmp/work/MyApp/ATCar.cs(2,17): warning CS0108: 'ATCar.Accelerator()' は継承されたメンバー 'Car.Accelerator()' を非表示にします。非表示にする場合は、キーワード new を使用してください。 [/tmp/work/MyApp/MyApp.csproj]
```

## MTCar.cs

```csharp
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
```

　MT車。`Car`を継承する。`Accelerator()`をオーバーライドではなくオーバーロードする。引数がちがう同名メソッドはオーバーロードになる。

　オーバーロードの場合、同名だが別メソッドとして識別されるらしい。よって`new`をつける必要はない。つまり、`MTCar`クラスは`Accelerator()`と`Accelerator(int)`の2種類の異なるメソッドを保持していることになる。

　私としてはMTの場合`Accelerator()`のほうは不要だった。`Accelerator(int)`だけ実装したかった。むしろ`Accelerator()`が呼び出されたら「そんなメソッドないよ！」とコンパイラ先生に叱ってもらいたかった。

　だったら継承だのインタフェースだのは使わずにAT、MTそれぞれ別クラスにすべきだったのかな？　でもアクセルを踏むという共通のメソッドは欲しいし。うーん。

## Program.cs

### インスタンス生成

　各クラスのインスタンスを生成する。

```csharp
Car car = new Car();
ATCar at = new ATCar();
MTCar mt = new MTCar();
```

### Car

　まずはギアの実装がなにもない`Car`クラスで`Accelerator()`を呼ぶ。

```csharp
car.Accelerator();
car.Accelerator();
```

　実行結果は以下。ギアは`0`である。

```sh
Car.Accelerator() 0
Car.Accelerator() 0
```

### AT

　つぎにAT車のそれを呼ぶ。

```csharp
at.Accelerator();
at.Accelerator();
at.Accelerator();
at.Accelerator();
at.Accelerator();
at.Accelerator();
at.Accelerator();
```

　実行結果は以下。ギアは`1`〜`6`である。最大値`6`までいったら`1`に戻る。このAT車、いちど走り出したら止まれない仕様である。おそろしい。

```sh
ATCar.Accelerator() 1
ATCar.Accelerator() 2
ATCar.Accelerator() 3
ATCar.Accelerator() 4
ATCar.Accelerator() 5
ATCar.Accelerator() 6
ATCar.Accelerator() 1
```

### MT

　MT車。まずは引数なしから。これが呼べてしまう。`Car`クラスのメソッドである。私の気持ちとして引数なしのときは「そんなメソッドはないよ！」と叱ってほしいので、この実装はベストじゃない。どうしたらいいんだろう。

```csharp
mt.Accelerator();
mt.Accelerator();
```

```sh
Car.Accelerator() 0
Car.Accelerator() 0
```

　つぎに本命、MT車でギアを渡す。

```csharp
mt.Accelerator(1);
mt.Accelerator(2);
mt.Accelerator(3);
mt.Accelerator(2);
mt.Accelerator(4); // 2から4に一気にあげたからエンストする
mt.Accelerator(3); // 一度エンストするとギアは0にもどる。0との差が1でないからやはりエンストする
mt.Accelerator(1); // エンストから復帰するには1を渡すしかない
```
```sh
MTCar.Accelerator(1) 0
MTCar.Accelerator(2) 1
MTCar.Accelerator(3) 2
MTCar.Accelerator(2) 3
MTCar.Accelerator(4) 2 エンスト！
MTCar.Accelerator(3) 0 エンスト！
MTCar.Accelerator(1) 0
```

　AT, MTインスタンス変数の型を親クラス`Car`に変えてみる。

```csharp
Car at1 = new ATCar();
Car mt1 = new MTCar();
at1.Accelerator();
at1.Accelerator();
//mt1.Accelerator(1); // error CS1501: 引数 1 を指定するメソッド 'Accelerator' のオーバーロードはありません
//mt1.Accelerator(2); //  error CS1501: 引数 1 を指定するメソッド 'Accelerator' のオーバーロードはありません
```
```sh
ATCar.Accelerator() 0
ATCar.Accelerator() 0
```

　AT, MTインスタンス変数の型をインタフェース`IAccelerator`に変えてみる。

```csharp
IAccelerator at2 = new ATCar();
IAccelerator mt2 = new MTCar();
at2.Accelerator();
at2.Accelerator();
//mt2.Accelerator(1); // error CS1501: 引数 1 を指定するメソッド 'Accelerator' のオーバーロードはありません
//mt2.Accelerator(2); // error CS1501: 引数 1 を指定するメソッド 'Accelerator' のオーバーロードはありません
```
```sh
ATCar.Accelerator() 0
ATCar.Accelerator() 0
```

　私としては次のようなイメージだった。

```csharp
IAccelerator car = generateCar(); // ATまたはMTを返す
car.Accelerator(1) // AT/MTどちらでもエラーなく動く
```

　引数は必ずある想定。引数を受けとらないATのときは引数を無視する。

　でも、そうはならなかった。原因は以下だと思う。

* 引数の有無によってメソッドシグネチャが変わる
* 引数ありのシグネチャを用意していない

　これらを解決するには`IAccelerator`の`Accelerator()`を引数なしとありの2パターンで通用する書き方をする必要がありそう。

方法|コード例
----|--------
オプション引数|`Accelerator(int gear = -1)`
名前付き引数呼出|`Accelerator(gear : 3)`
可変長引数|`Accelerator(params int[] gear)`

　次はこれで試してみよう。

















# 期待する実装案

パターン|のぞむ結果
--------|----------
`Car.Accelerator()`|そもそも`Car`をインスタンス化したくない！
`Car.Accelerator(1)`|そもそも`Car`をインスタンス化したくない！
`ATCar.Accelerator()`|OK！
`ATCar.Accelerator(1)`|そんなメソッドはないよ！
`MTCar.Accelerator()`|そんなメソッドはないよ！
`MTCar.Accelerator(1)`|OK！

　どうすれば上のようにできるか。アイデアは以下。

* `Accelerator()`をオプション引数にする
* `Accelerator()`を名前付き引数にする
* `Accelerator()`を可変長引数にする

　でも、これを使ってもムリでは？　今度はATのほうで引数ありが呼び出せるようになってしまう。なら、その場合は例外を投げればいいのかな？

* `Accelerator()`をオプション引数にする
	* MTで引数なしの場合、例外を発生させる
	* ATで引数ありの場合、例外を発生させる

　……なんか、ダサくない？　これでいいのかな。もっとクールに書けないのか。

　たぶんオブジェクト指向のうち何か大事な概念があって、それを使えば解決できる気がする。`virtual`, `override`というキーワードが臭う。

* [仮想／抽象／インタフェースを使い分けるには？][]

[仮想／抽象／インタフェースを使い分けるには？]:https://atmarkit.itmedia.co.jp/ait/articles/1801/10/news017.html

種別|ちがい|キーワード
----|------|----------
仮想メソッド|既定の実装を与える|`virtual`, `override`
抽象クラス|既定の実装を与えない|`abstract class`
インタフェース|既定の実装を与えない|`interface`

メソッドパターン|問題
----------------|----
`Accelerator()`|引数ありのとは別のシグネチャになってしまう
`Accelerator(int gear = -1)`|

IAccelerator 

# 所感

　できるだろうと思っていたが、実際に書いてみると思ってたことができなかった。

