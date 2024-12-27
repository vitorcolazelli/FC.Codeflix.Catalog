using FC.Codeflix.Catalog.Domain.SeedWork;
using FC.Codeflix.Catalog.Domain.Validation;
using FC.Codeflix.Catalog.Domain.Validator;

namespace FC.Codeflix.Catalog.Domain.Entity;

public class Video : AggregateRoot
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public int YearLaunched { get; private set; }
    public bool Opened { get; private set; }
    public bool Published { get; private set; }
    public int Duration { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    public void Validate(ValidationHandler handler)
        => new VideoValidator(this, handler).Validate();
    
    public Video(
        string title, 
        string description, 
        int yearLaunched, 
        bool opened, 
        bool published, 
        int duration)
    {
        Title = title;
        Description = description;
        YearLaunched = yearLaunched;
        Opened = opened;
        Published = published;
        Duration = duration;
        CreatedAt = DateTime.Now;
    }
}